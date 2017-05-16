using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    /// <summary>
    /// The purpose of this rewriter is to replace await-containing catch and finally handlers
    /// with surrogate replacements that keep actual handler code in regular code blocks.
    /// That allows these constructs to be further lowered at the async lowering pass.
    ///
    /// Adapted from Microsoft.CodeAnalysis.CSharp.AsyncExceptionHandlerRewriter in the Roslyn codebase
    /// </summary>
    public sealed class AsyncExceptionHandlerRewriter : DepthFirstTransformer
    {
        private static readonly IMethod _exceptionDispatchInfoCapture;
        private static readonly IMethod _exceptionDispatchInfoThrow;

        private readonly BooCodeBuilder _F;
        private readonly TypeSystemServices _tss;
        private readonly AwaitInFinallyAnalysis _analysis;
        private readonly Method _containingMethod;

        private AwaitCatchFrame _currentAwaitCatchFrame;
        private AwaitFinallyFrame _currentAwaitFinallyFrame;

		private int _tryDepth = 1;

        static AsyncExceptionHandlerRewriter()
        {
            var tss = My<TypeSystemServices>.Instance;
            var exceptionDispatchInfo = tss.Map(typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo));
            var methods = exceptionDispatchInfo.GetMembers().OfType<IMethod>().ToArray();
            _exceptionDispatchInfoCapture = methods.SingleOrDefault(m => m.Name.Equals("Capture"));
            _exceptionDispatchInfoThrow = methods.SingleOrDefault(m => m.Name.Equals("Throw"));
        }

        private AsyncExceptionHandlerRewriter(
            Method containingMethod,
            BooCodeBuilder factory,
            AwaitInFinallyAnalysis analysis)
        {
            _F = factory;
            _analysis = analysis;
            _containingMethod = containingMethod;
            _currentAwaitFinallyFrame = new AwaitFinallyFrame(factory);
            _tss = My<TypeSystemServices>.Instance;
        }

        /// <summary>
        /// Lower a block of code by performing local rewritings. 
        /// The goal is to not have exception handlers that contain awaits in them.
        /// 
        /// 1) Await containing ensure blocks:
        ///     The general strategy is to rewrite await containing handlers into synthetic handlers.
        ///     Synthetic handlers are not handlers in IL sense so it is ok to have awaits in them.
        ///     Since synthetic handlers are just blocks, we have to deal with pending exception/branch/return manually
        ///     (this is the hard part of the rewrite).
        ///
        ///     try:
        ///        code
        ///     ensure:
        ///        handler
        ///
        /// Into ===>
        ///
        ///     ex as Exception = null
        ///     pendingBranch as int = 0
        ///
        ///     try:
        ///         code  // any gotos/returns are rewritten to code that pends the necessary info and goes to finallyLabel
        ///         goto finallyLabel
        ///     except ex:  // essentially pend the currently active exception
        ///         pass
        /// 
        ///     :finallyLabel
        ///        handler
        ///        if ex != null: raise ex     // unpend the exception
        ///        unpend branches/return
        /// 
        /// 2) Await containing catches:
        ///     try:
        ///         code
        ///     except ex as Exception:
        ///         handler
        ///         raise
        /// 
        /// 
        /// Into ===>
        ///
        ///     pendingException as Object
        ///     pendingCatch as int = 0
        ///
        ///     try:
        ///         code
        ///     except temp as Exception:  // essentially pend the currently active exception
        ///         pendingException = temp
        ///         pendingCatch = 1
        ///
        ///     if pendingCatch == 1:
        ///             var ex = pendingException cast Exception
        ///             handler
        ///             raise pendingException
        /// </summary>
        public static void Rewrite(Method containingMethod)
        {
            if (containingMethod == null) throw new ArgumentNullException("containingMethod");

            var body = containingMethod.Body;
            var analysis = new AwaitInFinallyAnalysis(body);
            if (analysis.ContainsAwaitInHandlers())
            {
                var factory = CompilerContext.Current.CodeBuilder;
                var rewriter = new AsyncExceptionHandlerRewriter(containingMethod, factory, analysis);
                containingMethod.Body = (Block)rewriter.Visit(body);
            }
        }

        public override void OnTryStatement(TryStatement node)
        {
            Statement finalizedRegion;
            Block rewrittenFinally;

            var finallyContainsAwaits = _analysis.FinallyContainsAwaits(node);
            if (!finallyContainsAwaits)
            {
                finalizedRegion = RewriteFinalizedRegion(node);
				++_tryDepth;
                rewrittenFinally = (Block)Visit(node.EnsureBlock);
				--_tryDepth;

                if (rewrittenFinally == null)
                {
                    ReplaceCurrentNode(finalizedRegion);
                    return;
                }

                var asTry = finalizedRegion as TryStatement;
                if (asTry != null)
                {
                    // since finalized region is a try we can just attach finally to it
                    Debug.Assert(asTry.EnsureBlock == null);
                    asTry.EnsureBlock = rewrittenFinally;
                    ReplaceCurrentNode(asTry);
                    return;
                }
                // wrap finalizedRegion into a Try with a finally.
                ReplaceCurrentNode(new TryStatement(finalizedRegion.LexicalInfo) {EnsureBlock = rewrittenFinally});
                return;
            }

            // rewrite finalized region (try and catches) in the current frame
            var frame = PushFrame(node);
            finalizedRegion = RewriteFinalizedRegion(node);
            rewrittenFinally = (Block)Visit(node.EnsureBlock);
            PopFrame();

            var context = CompilerContext.Current;
            var exceptionType = _tss.ObjectType;
            var pendingExceptionLocal = _F.DeclareTempLocal(_containingMethod, exceptionType);
            var finallyLabel = _F.CreateLabel(node.EnsureBlock, context.GetUniqueName("finallyLabel"), _tryDepth);
            var pendingBranchVar =_F.DeclareTempLocal(_containingMethod, _tss.IntType);

            var catchAll = new ExceptionHandler
            {
                Declaration = new Declaration(pendingExceptionLocal.Name, _F.CreateTypeReference(exceptionType))
                    {Entity = pendingExceptionLocal},
                Block = new Block(),
                IsSynthetic = true
            };

            var tryBlock = new Block(
                    finalizedRegion,
                    _F.CreateGoto(finallyLabel, _tryDepth),
                    PendBranches(frame, pendingBranchVar, finallyLabel))
            ;
            var catchAndPendException = new TryStatement {ProtectedBlock = tryBlock};
            catchAndPendException.ExceptionHandlers.Add(catchAll);

            var syntheticFinally = new Block(
                finallyLabel.LabelStatement,
                rewrittenFinally,
                UnpendException(pendingExceptionLocal),
                UnpendBranches(
                    frame,
                    pendingBranchVar,
                    pendingExceptionLocal));

            var locals = _containingMethod.Locals;
            var statements = new Block();

            locals.Add(pendingExceptionLocal.Local);
            statements.Add(_F.CreateAssignment(
                _F.CreateLocalReference(pendingExceptionLocal),
                _F.CreateDefaultInvocation(LexicalInfo.Empty, pendingExceptionLocal.Type)));
            locals.Add(pendingBranchVar.Local);
            statements.Add(_F.CreateAssignment(
                _F.CreateLocalReference(pendingBranchVar),
                _F.CreateDefaultInvocation(LexicalInfo.Empty, pendingBranchVar.Type)));

            var returnLocal = frame.returnValue;
            if (returnLocal != null)
            {
                locals.Add(returnLocal.Local);
            }

            statements.Add(catchAndPendException);
            statements.Add(syntheticFinally);

            ReplaceCurrentNode(statements);
        }

        private Block PendBranches(
            AwaitFinallyFrame frame,
            InternalLocal pendingBranchVar,
            InternalLabel finallyLabel)
        {
            var bodyStatements = new Block();

            // handle proxy labels if have any
            var proxiedLabels = frame.proxiedLabels;
            var proxyLabels = frame.proxyLabels;

            // skip 0 - it means we took no explicit branches
            int i = 1;
            if (proxiedLabels != null)
            {
                for (int cnt = proxiedLabels.Count; i <= cnt; i++)
                {
                    var proxied = proxiedLabels[i - 1];
                    var proxy = proxyLabels[proxied];

                    PendBranch(bodyStatements, proxy, i, pendingBranchVar, finallyLabel);
                }
            }

            var returnProxy = frame.returnProxyLabel;
            if (returnProxy != null)
            {
                PendBranch(bodyStatements, returnProxy, i, pendingBranchVar, finallyLabel);
            }

            return bodyStatements;
        }

        private void PendBranch(
            Block bodyStatements,
            InternalLabel proxy,
            int i,
            InternalLocal pendingBranchVar,
            InternalLabel finallyLabel)
        {
            // branch lands here
            bodyStatements.Add(proxy.LabelStatement);

            // pend the branch
            bodyStatements.Add(_F.CreateAssignment(
                _F.CreateLocalReference(pendingBranchVar),
                _F.CreateIntegerLiteral(i)));

            // skip other proxies
			bodyStatements.Add(_F.CreateGoto(finallyLabel, _tryDepth));
        }

        private Statement UnpendBranches(
            AwaitFinallyFrame frame,
            InternalLocal pendingBranchVar,
            InternalLocal pendingException)
        {
            var parent = frame.ParentOpt;

            // handle proxy labels if have any
            var proxiedLabels = frame.proxiedLabels;

            // skip 0 - it means we took no explicit branches
            int i = 1;
            var cases = new List<Statement>();

            if (proxiedLabels != null)
            {
                for (int cnt = proxiedLabels.Count; i <= cnt; i++)
                {
                    var target = proxiedLabels[i - 1];
                    var parentProxy = parent.ProxyLabelIfNeeded(target);
					cases.Add(_F.CreateGoto(parentProxy, _tryDepth));
                }
            }

            if (frame.returnProxyLabel != null)
            {
                Local pendingValue = null;
                if (frame.returnValue != null)
                {
                    pendingValue = frame.returnValue.Local;
                }

                InternalLocal returnValue;
                Statement unpendReturn;

                var returnLabel = parent.ProxyReturnIfNeeded(_containingMethod, pendingValue, out returnValue);

                if (returnLabel == null)
                {
                    unpendReturn = new ReturnStatement(_F.CreateLocalReference((InternalLocal)pendingValue.Entity));
                }
                else
                {
                    if (pendingValue == null)
                    {
						unpendReturn = _F.CreateGoto(returnLabel, _tryDepth);
                    }
                    else
                    {
                        unpendReturn = new Block(
                            new ExpressionStatement(
                                _F.CreateAssignment(_F.CreateLocalReference(returnValue),
                                    _F.CreateLocalReference((InternalLocal)pendingValue.Entity))),
							_F.CreateGoto(returnLabel, _tryDepth));
                    }
                }

                cases.Add(unpendReturn);
            }

            var defaultLabel = _F.CreateLabel(_containingMethod, CompilerContext.Current.GetUniqueName("default"), _tryDepth);
			cases.Insert(0, _F.CreateGoto(defaultLabel, _tryDepth));
            return CreateSwitch(cases, defaultLabel, pendingBranchVar);
        }

        private Block SwitchBlock(Statement body, int ordinal, InternalLabel endpoint)
        {
            var result = new Block();
			result.Add(_F.CreateLabel(result, CompilerContext.Current.GetUniqueName("L" + ordinal), _tryDepth).LabelStatement);
            result.Add(body);
			result.Add(_F.CreateGoto(endpoint, _tryDepth));
            return result;
        }

        private Block CreateSwitch(List<Statement> handlers, InternalLabel endLabel, InternalLocal switchVar)
        {
            var blocks = handlers.Zip(Enumerable.Range(1, handlers.Count), (s, i1) => SwitchBlock(s, i1, endLabel)).ToArray();
            var resultSwitch = _F.CreateSwitch(_F.CreateLocalReference(switchVar),
                blocks.Select(b => b.FirstStatement).Cast<LabelStatement>());
            var result = new Block(resultSwitch);
            result.Statements.AddRange(blocks);
            result.Statements.Add(endLabel.LabelStatement);
            return result;
        }

        public override void OnGotoStatement(GotoStatement node)
        {
            base.OnGotoStatement(node);
            var proxyLabel = _currentAwaitFinallyFrame.ProxyLabelIfNeeded((InternalLabel) node.Label.Entity);
            node.Label.Entity = proxyLabel;
        }

        public override void OnReturnStatement(ReturnStatement node)
        {
            InternalLocal returnValue;
            var returnLabel = _currentAwaitFinallyFrame.ProxyReturnIfNeeded(
                _containingMethod,
                node.Expression,
                out returnValue);

            if (returnLabel == null)
            {
                base.OnReturnStatement(node);
                return;
            }

            var returnExpr = Visit(node.Expression);
            Statement result;
            if (returnExpr != null)
            {
                result = new Block(
                        new ExpressionStatement(
                            _F.CreateAssignment(
                                _F.CreateLocalReference(returnValue),
                                returnExpr)),
						_F.CreateGoto(returnLabel, _tryDepth));
            }
            else
            {
				result = _F.CreateGoto(returnLabel, _tryDepth);
            }
            ReplaceCurrentNode(result);
        }

        private Statement UnpendException(InternalLocal pendingExceptionLocal)
        {
            // create a temp. 
            // pendingExceptionLocal will certainly be captured, no need to access it over and over.
            InternalLocal obj = _F.DeclareTempLocal(_containingMethod, _tss.ObjectType);
            var objInit = _F.CreateAssignment(_F.CreateLocalReference(obj), _F.CreateLocalReference(pendingExceptionLocal));

            // throw pendingExceptionLocal;
            Statement rethrow = Rethrow(obj);

            return new Block(
                new ExpressionStatement(objInit),
                new IfStatement(
                    pendingExceptionLocal.Local.LexicalInfo,
                    _F.CreateBoundBinaryExpression(
                        _tss.BoolType,
                        BinaryOperatorType.ReferenceInequality, 
                        _F.CreateLocalReference(obj),
                        new NullLiteralExpression()),
                    new Block(rethrow),
                    null));
        }

        private Statement Rethrow(InternalLocal obj)
        {
            // conservative rethrow 
            Statement rethrow = new RaiseStatement(_F.CreateLocalReference(obj));

            // if these helpers are available, we can rethrow with original stack info
            // as long as it derives from Exception
            if (_exceptionDispatchInfoCapture != null && _exceptionDispatchInfoThrow != null)
            {
                var ex = _F.DeclareTempLocal(_containingMethod, _tss.ExceptionType);
                var assignment = _F.CreateAssignment(
                    _F.CreateLocalReference(ex),
                    _F.CreateAsCast(ex.Type, _F.CreateLocalReference(obj)));

                // better rethrow 
                rethrow = new Block(
                    new ExpressionStatement(assignment),
                    new IfStatement(
                        _F.CreateBoundBinaryExpression(
                            _tss.BoolType,
                            BinaryOperatorType.ReferenceEquality,
                            _F.CreateLocalReference(ex),
                            new NullLiteralExpression()),
                        new Block(rethrow),
                        null),
                    // ExceptionDispatchInfo.Capture(pendingExceptionLocal).Throw()
                    new ExpressionStatement(
                        _F.CreateMethodInvocation(
                            _F.CreateMethodInvocation(
                                _exceptionDispatchInfoCapture,
                                _F.CreateLocalReference(ex)),
                            _exceptionDispatchInfoThrow))
                );
            }

            return rethrow;
        }

        /// <summary>
        /// Rewrites Try/Catch part of the Try/Catch/Finally
        /// </summary>
        private Statement RewriteFinalizedRegion(TryStatement node)
        {
            var rewrittenTry = (Block)Visit(node.ProtectedBlock);

            var catches = node.ExceptionHandlers;
            if (catches.IsEmpty)
            {
                return rewrittenTry;
            }

            var origAwaitCatchFrame = _currentAwaitCatchFrame;
            _currentAwaitCatchFrame = null;

            Visit(node.ExceptionHandlers);
            Statement tryWithCatches = new TryStatement{EnsureBlock = rewrittenTry};
            ((TryStatement)tryWithCatches).ExceptionHandlers = node.ExceptionHandlers;

            var currentAwaitCatchFrame = _currentAwaitCatchFrame;
            if (currentAwaitCatchFrame != null)
            {
				var handledLabel = _F.CreateLabel(tryWithCatches, CompilerContext.Current.GetUniqueName("handled"), _tryDepth);
                var handlersList = currentAwaitCatchFrame.handlers;
				_tryDepth = node.GetAncestors<TryStatement>().Count() + 1;
				var handlers = new List<Statement> { _F.CreateGoto(handledLabel, _tryDepth) };
                for (int i = 0, l = handlersList.Count; i < l; i++)
                {
                    handlers.Add(
                        new Block(
                            handlersList[i],
							_F.CreateGoto(handledLabel, _tryDepth)));
                }

                _containingMethod.Locals.Add(currentAwaitCatchFrame.pendingCaughtException.Local);
                _containingMethod.Locals.Add(currentAwaitCatchFrame.pendingCatch.Local);
                _containingMethod.Locals.AddRange(currentAwaitCatchFrame.GetHoistedLocals().Select(l => l.Local));

                tryWithCatches = new Block(
                    new ExpressionStatement(
                        _F.CreateAssignment(
                            _F.CreateLocalReference(currentAwaitCatchFrame.pendingCatch),
                            _F.CreateDefaultInvocation(LexicalInfo.Empty, currentAwaitCatchFrame.pendingCatch.Type))),
                    tryWithCatches,
                    CreateSwitch(handlers, handledLabel, currentAwaitCatchFrame.pendingCatch));
            }

            _currentAwaitCatchFrame = origAwaitCatchFrame;

            return tryWithCatches;
        }

        public override void OnExceptionHandler(ExceptionHandler node)
        {
            if (!_analysis.CatchContainsAwait(node))
            {
                var origCurrentAwaitCatchFrame = _currentAwaitCatchFrame;
                _currentAwaitCatchFrame = null;

                var result = Visit(node);
                _currentAwaitCatchFrame = origCurrentAwaitCatchFrame;
                ReplaceCurrentNode(result);
            }

            var currentAwaitCatchFrame = _currentAwaitCatchFrame ??
                (_currentAwaitCatchFrame = new AwaitCatchFrame(_tss, _F, _containingMethod));

            var catchType = node.Declaration != null ? (IType)node.Declaration.Type.Entity : _tss.ObjectType;
            var catchTemp = _F.DeclareTempLocal(_containingMethod, catchType);

            var storePending = _F.CreateAssignment(
                        _F.CreateLocalReference(currentAwaitCatchFrame.pendingCaughtException),
                        _F.CreateCast(currentAwaitCatchFrame.pendingCaughtException.Type,
                            _F.CreateLocalReference(catchTemp)));

            var setPendingCatchNum = _F.CreateAssignment(
                            _F.CreateLocalReference(currentAwaitCatchFrame.pendingCatch),
                            _F.CreateIntegerLiteral(currentAwaitCatchFrame.handlers.Count + 1));

            //  catch (ExType exTemp)
            //  {
            //      pendingCaughtException = exTemp;
            //      catchNo = X;
            //  }
            ExceptionHandler catchAndPend;

            var filterOpt = node.FilterCondition;
            if (filterOpt == null)
            {
                // store pending exception 
                // as the first statement in a catch
                catchAndPend = new ExceptionHandler
                {
                    Declaration = new Declaration(catchTemp.Name, _F.CreateTypeReference(catchType)) { Entity = catchTemp },
                    Block = new Block(
                        new ExpressionStatement(storePending),
                        new ExpressionStatement(setPendingCatchNum))
                };
                // catch locals live on the synthetic catch handler block
            }
            else
            {
                // catch locals move up into hoisted locals
                // since we might need to access them from both the filter and the catch
                foreach (var local in LocalsUsedIn(node))
                {
                    currentAwaitCatchFrame.HoistLocal(local, _F);
                }

                // store pending exception 
                // as the first expression in a filter
                var decl = node.Declaration;
                var sourceOpt = decl != null && !string.IsNullOrEmpty(decl.Name) ? decl : null;
                var rewrittenFilter = Visit(filterOpt);
                var newFilter = sourceOpt == null ?
                                _F.CreateEvalInvocation(
                                    storePending.LexicalInfo,
                                    storePending,
                                    rewrittenFilter) :
                                _F.CreateEvalInvocation(
                                    storePending.LexicalInfo,
                                    storePending,
                                    AssignCatchSource((Declaration)Visit(sourceOpt), currentAwaitCatchFrame),
                                    rewrittenFilter);

                catchAndPend = new ExceptionHandler
                {
                    Declaration = new Declaration(catchTemp.Name, _F.CreateTypeReference(catchType)) { Entity = catchTemp },
                    FilterCondition = newFilter,
                    Block = new Block(new ExpressionStatement(setPendingCatchNum))
                };
            }
            if (node.ContainsAnnotation("isSynthesizedAsyncCatchAll"))
                catchAndPend.Annotate("isSynthesizedAsyncCatchAll");

            var handlerStatements = new List<Statement>();

            if (filterOpt == null)
            {
                var sourceOpt = node.Declaration;
                if (sourceOpt != null && sourceOpt.Entity != null)
                {
                    Expression assignSource = AssignCatchSource((Declaration)Visit(sourceOpt), currentAwaitCatchFrame);
                    handlerStatements.Add(new ExpressionStatement(assignSource));
                }
            }

            handlerStatements.Add(Visit(node.Block));

            var handler = new Block(handlerStatements.ToArray());

            currentAwaitCatchFrame.handlers.Add(handler);

            ReplaceCurrentNode(catchAndPend);
        }

        private IEnumerable<InternalLocal> LocalsUsedIn(ExceptionHandler handler)
        {
            var rc = new LocalReferenceCollector(_containingMethod.Locals.Select(l => (InternalLocal) l.Entity));
            handler.Accept(rc);
            return rc.Result;
        }

        private Expression AssignCatchSource(Declaration rewrittenSource, AwaitCatchFrame currentAwaitCatchFrame)
        {
            Expression assignSource = null;
            if (rewrittenSource != null)
            {
                // exceptionSource = (exceptionSourceType)pendingCaughtException;
                assignSource = _F.CreateAssignment(
                                    _F.CreateLocalReference((InternalLocal)rewrittenSource.Entity),
                                    _F.CreateCast(
                                        (IType) rewrittenSource.Type.Entity,
                                        _F.CreateLocalReference(currentAwaitCatchFrame.pendingCaughtException)));
            }

            return assignSource;
        }

        public override void OnDeclaration(Declaration node)
        {
			if (node.Entity == null)
				return;
            var catchFrame = _currentAwaitCatchFrame;
            InternalLocal hoistedLocal = null;
            if (catchFrame == null || !catchFrame.TryGetHoistedLocal((InternalLocal)node.Entity, out hoistedLocal))
            {
                base.OnDeclaration(node);
            }

            ReplaceCurrentNode(new Declaration(node.Name, _F.CreateTypeReference(hoistedLocal.Type)) {Entity = hoistedLocal});
        }

        public override void OnRaiseStatement(RaiseStatement node)
        {
            if (node.Exception != null || _currentAwaitCatchFrame == null)
            {
                base.OnRaiseStatement(node);
            }

            ReplaceCurrentNode(Rethrow(_currentAwaitCatchFrame.pendingCaughtException));
        }

        public override void OnBlockExpression(BlockExpression node)
        {
            throw new NotImplementedException(); //should have been rewritten already
        }

        private AwaitFinallyFrame PushFrame(TryStatement statement)
        {
            var newFrame = new AwaitFinallyFrame(_currentAwaitFinallyFrame, _analysis.Labels(statement), statement, _F, _tryDepth);
            _currentAwaitFinallyFrame = newFrame;
            return newFrame;
        }

        private void PopFrame()
        {
            var result = _currentAwaitFinallyFrame;
            _currentAwaitFinallyFrame = result.ParentOpt;
        }

        /// <summary>
        /// Analyzes method body for try blocks with awaits in finally blocks 
        /// Also collects labels that such blocks contain.
        /// </summary>
        private sealed class AwaitInFinallyAnalysis : LabelCollector
        {
            // all try blocks with yields in them and complete set of labels inside those try blocks
            // NOTE: non-yielding try blocks are transparently ignored - i.e. their labels are included
            //       in the label set of the nearest yielding-try parent  
            private Dictionary<TryStatement, HashSet<InternalLabel>> _labelsInInterestingTry;

            private HashSet<ExceptionHandler> _awaitContainingCatches;

            // transient accumulators.
            private bool _seenAwait;

            public AwaitInFinallyAnalysis(Statement body)
            {
                _seenAwait = false;
                Visit(body);
            }

            /// <summary>
            /// Returns true if a finally of the given try contains awaits
            /// </summary>
            public bool FinallyContainsAwaits(TryStatement statement)
            {
                return _labelsInInterestingTry != null && _labelsInInterestingTry.ContainsKey(statement);
            }

            /// <summary>
            /// Returns true if a catch contains awaits
            /// </summary>
            internal bool CatchContainsAwait(ExceptionHandler node)
            {
                return _awaitContainingCatches != null && _awaitContainingCatches.Contains(node);
            }

            /// <summary>
            /// Returns true if body contains await in a finally block.
            /// </summary>
            public bool ContainsAwaitInHandlers()
            {
                return _labelsInInterestingTry != null || _awaitContainingCatches != null;
            }

            /// <summary>
            /// Labels reachable from within this frame without invoking its finally. 
            /// null if there are no such labels.
            /// </summary>
            internal HashSet<InternalLabel> Labels(TryStatement statement)
            {
                return _labelsInInterestingTry[statement];
            }

            public override void OnTryStatement(TryStatement node)
            {
                var origLabels = _currentLabels;
                _currentLabels = null;
                Visit(node.ProtectedBlock);
                Visit(node.ExceptionHandlers);

                var origSeenAwait = _seenAwait;
                _seenAwait = false;
                Visit(node.EnsureBlock);

                if (_seenAwait)
                {
                    // this try has awaits in the finally !
                    var labelsInInterestingTry = _labelsInInterestingTry;
                    if (labelsInInterestingTry == null)
                    {
                        _labelsInInterestingTry = labelsInInterestingTry = new Dictionary<TryStatement, HashSet<InternalLabel>>();
                    }

                    labelsInInterestingTry.Add(node, _currentLabels);
                    _currentLabels = origLabels;
                }
                else
                {
                    // this is a boring try without awaits in finally

                    // currentLabels = currentLabels U origLabels ;
                    if (_currentLabels == null)
                    {
                        _currentLabels = origLabels;
                    }
                    else if (origLabels != null)
                    {
                        _currentLabels.UnionWith(origLabels);
                    }
                }

                _seenAwait = _seenAwait | origSeenAwait;
            }

            public override void OnExceptionHandler(ExceptionHandler node)
            {
                var origSeenAwait = _seenAwait;
                _seenAwait = false;

                base.OnExceptionHandler(node);

                if (_seenAwait)
                {
                    if (_awaitContainingCatches == null)
                    {
                        _awaitContainingCatches = new HashSet<ExceptionHandler>();
                    }

                    _awaitContainingCatches.Add(node);
                }

                _seenAwait |= origSeenAwait;
            }

            public override void OnAwaitExpression(AwaitExpression node)
            {
                _seenAwait = true;
                base.OnAwaitExpression(node);
            }
        }

        // storage of various information about a given finally frame
        private sealed class AwaitFinallyFrame
        {
            // Enclosing frame. Root frame does not have parent.
            public readonly AwaitFinallyFrame ParentOpt;

            // labels within this frame (branching to these labels does not go through finally).
            private readonly HashSet<InternalLabel> _labelsOpt;

            private readonly BooCodeBuilder _builder;

            // the try statement the frame is associated with
            private readonly TryStatement _tryStatementOpt;

            // proxy labels for branches leaving the frame. 
            // we build this on demand once we encounter leaving branches.
            // subsequent leaves to an already proxied label redirected to the proxy.
            // At the proxy label we will execute finally and forward the control flow 
            // to the actual destination. (which could be proxied again in the parent)
            public Dictionary<InternalLabel, InternalLabel> proxyLabels;

            public List<InternalLabel> proxiedLabels;

            public InternalLabel returnProxyLabel;
            public InternalLocal returnValue;

	        private int _tryDepth;

            public AwaitFinallyFrame(BooCodeBuilder builder)
            {
                _builder = builder;
            }

            public AwaitFinallyFrame(AwaitFinallyFrame parent, HashSet<InternalLabel> labelsOpt, TryStatement TryStatement, BooCodeBuilder builder, int depth)
            {
                Debug.Assert(parent != null);
                Debug.Assert(TryStatement != null);

                ParentOpt = parent;
                _labelsOpt = labelsOpt;
                _tryStatementOpt = TryStatement;
                _builder = builder;
	            _tryDepth = depth;
            }

            private bool IsRoot()
            {
                return ParentOpt == null;
            }

            // returns a proxy for a label if branch must be hijacked to run finally
            // otherwise returns same label back
            public InternalLabel ProxyLabelIfNeeded(InternalLabel label)
            {
                // no need to proxy a label in the current frame or when we are at the root
                if (IsRoot() || (_labelsOpt != null && _labelsOpt.Contains(label)))
                {
                    return label;
                }

                var proxyLabels = this.proxyLabels;
                var proxiedLabels = this.proxiedLabels;
                if (proxyLabels == null)
                {
                    this.proxyLabels = proxyLabels = new Dictionary<InternalLabel, InternalLabel>();
                    this.proxiedLabels = proxiedLabels = new List<InternalLabel>();
                }

                InternalLabel proxy;
                if (!proxyLabels.TryGetValue(label, out proxy))
                {
					proxy = _builder.CreateLabel(label.LabelStatement, "proxy" + label.Name, _tryDepth);
                    proxyLabels.Add(label, proxy);
                    proxiedLabels.Add(label);
                }

                return proxy;
            }

            public InternalLabel ProxyReturnIfNeeded(
                Method containingMethod,
                Node valueOpt,
                out InternalLocal retVal)
            {
                retVal = null;

                // no need to proxy returns  at the root
                if (IsRoot())
                {
                    return null;
                }

                var returnProxy = returnProxyLabel;
                if (returnProxy == null)
                {
					returnProxyLabel = returnProxy = _builder.CreateLabel(valueOpt ?? containingMethod, "returnProxy", _tryDepth);
                }

                if (valueOpt != null)
                {
                    retVal = returnValue;
                    if (retVal == null)
                    {
                        Debug.Assert(_tryStatementOpt != null);
                        returnValue = retVal = _builder.DeclareTempLocal(
                            containingMethod,
                            ((ITypedEntity)valueOpt.Entity).Type);
                    }
                }

                return returnProxy;
            }
        }

        private sealed class AwaitCatchFrame
        {
            // object, stores the original caught exception
            // used to initialize the exception source inside the handler
            // also used in rethrow statements
            public readonly InternalLocal pendingCaughtException;

            // int, stores the number of pending catch
            // 0 - means no catches are pending.
            public readonly InternalLocal pendingCatch;

            // synthetic handlers produced by catch rewrite.
            // they will become switch sections when pending exception is dispatched.
            public readonly List<Block> handlers;

            // when catch local must be used from a filter
            // we need to "hoist" it up to ensure that both the filter 
            // and the catch access the same variable.
            // NOTE: it must be the same variable, not just same value. 
            //       The difference would be observable if filter mutates the variable
            //       or/and if a variable gets lifted into a closure.
            private readonly Dictionary<InternalLocal, InternalLocal> _hoistedLocals;
            private readonly List<InternalLocal> _orderedHoistedLocals;

            private readonly Method _currentMethod;

            public AwaitCatchFrame(TypeSystemServices tss, BooCodeBuilder builder, Method currentMethod)
            {
                pendingCaughtException = builder.DeclareTempLocal(currentMethod, tss.ObjectType);
                pendingCatch = builder.DeclareTempLocal(currentMethod, tss.IntType);

                handlers = new List<Block>();
                _hoistedLocals = new Dictionary<InternalLocal, InternalLocal>();
                _orderedHoistedLocals = new List<InternalLocal>();
                _currentMethod = currentMethod;
            }

            public void HoistLocal(InternalLocal local, BooCodeBuilder F)
            {
                if (!_hoistedLocals.Keys.Any(l => l.Name == local.Name && l.Type == local.Type))
                {
                    _hoistedLocals.Add(local, local);
                    _orderedHoistedLocals.Add(local);
                    return;
                }

                // code uses "await" in two sibling catches with exception filters
                // locals with same names and types may cause problems if they are lifted
                // and become fields with identical signatures.
                // To avoid such problems we will mangle the name of the second local.
                // This will only affect debugging of this extremely rare case.
                var newLocal = F.DeclareTempLocal(_currentMethod, local.Type);

                _hoistedLocals.Add(local, newLocal);
                _orderedHoistedLocals.Add(newLocal);
            }

            public IEnumerable<InternalLocal> GetHoistedLocals()
            {
                return _orderedHoistedLocals;
            }

            public bool TryGetHoistedLocal(InternalLocal originalLocal, out InternalLocal hoistedLocal)
            {
                return _hoistedLocals.TryGetValue(originalLocal, out hoistedLocal);
            }
        }
    }
}
