using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Ast.Visitors;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Environments;

namespace Boo.Lang.Compiler.Steps.AsyncAwait
{
    using System.Collections.Generic;

    internal sealed class AwaitExpressionSpiller : DepthFirstTransformer
    {
        private const NodeType SpillSequenceBuilder = NodeType.CustomExpression; // NOTE: this node type is hijacked during this phase to represent BoundSpillSequenceBuilder

        private readonly BooCodeBuilder _F;
        private readonly Method _currentMethod;
        private readonly TypeSystemServices _tss;

        private AwaitExpressionSpiller(Method method, TypeSystemServices tss)
        {
            _F = My<BooCodeBuilder>.Instance;
            _currentMethod = method;
            _tss = tss;
        }

        private sealed class BoundSpillSequenceBuilder : CustomExpression
        {
            public readonly Expression Value;

            private List<InternalLocal> _locals;
            private List<Statement> _statements;

            public BoundSpillSequenceBuilder(Expression value = null)
            {
                Debug.Assert(value == null || value.NodeType != SpillSequenceBuilder);
                Value = value;
                ExpressionType = value == null ? null : value.ExpressionType;
            }

            public bool HasStatements
            {
                get
                {
                    return _statements != null;
                }
            }

            public bool HasLocals
            {
                get
                {
                    return _locals != null;
                }
            }

            public override void Accept(IAstVisitor visitor)
            {
                throw new InvalidOperationException("Should be unreachable");
            }

            public List<Statement> GetStatements()
            {
                return _statements;
            }

            internal BoundSpillSequenceBuilder Update(Expression value)
            {
                var result = new BoundSpillSequenceBuilder(value)
                {
                    _locals = _locals,
                    _statements = _statements
                };
                return result;
            }

            internal void Include(BoundSpillSequenceBuilder other)
            {
                if (other != null)
                {
                    Extend(ref _locals, ref other._locals);
                    Extend(ref _statements, ref other._statements);
                }
            }

            private static void Extend<T>(ref List<T> left, ref List<T> right)
            {
                if (right == null)
                {
                    return;
                }

                if (left == null)
                {
                    left = right;
                    return;
                }

                left.AddRange(right.Except(left));
            }

            public void AddLocal(InternalLocal local)
            {
                if (_locals == null)
                {
                    _locals = new List<InternalLocal>();
                }

                if (local.Type.IsRestrictedType())
                {
                    CompilerContext.Current.Errors.Add(
                        CompilerErrorFactory.RestrictedAwaitType(local.Local, local.Type));
                }

                _locals.Add(local);
            }

            public void AddStatement(Statement statement)
            {
                if (_statements == null)
                {
                    _statements = new List<Statement>();
                }

                _statements.Add(statement);
            }

            internal void AddExpressions(IEnumerable<Expression> expressions)
            {
				var existingHash = new HashSet<Expression>(_statements.OfType<ExpressionStatement>().Select(es => es.Expression));
                foreach (var expression in expressions)
                {
	                if (expression.NodeType == SpillSequenceBuilder)
	                {
		                var sb = (BoundSpillSequenceBuilder) expression;
						Include(sb);
						if (!existingHash.Contains(sb.Value))
							AddStatement(new ExpressionStatement(sb.LexicalInfo, sb.Value) { IsSynthetic = true });
	                }
                    else if (!existingHash.Contains(expression))
						AddStatement(new ExpressionStatement(expression.LexicalInfo, expression) { IsSynthetic = true });
                }
            }
        }

        internal static Statement Rewrite(Method method)
        {
            var spiller = new AwaitExpressionSpiller(method, My<TypeSystemServices>.Instance);
            var result = spiller.Visit(method.Body);
            return result;
        }

        private Expression VisitExpression(ref BoundSpillSequenceBuilder builder, Expression expression)
        {
            // wrap the node in a spill sequence to mark the fact that it must be moved up the tree.
            // The caller will handle this node type if the result is discarded.
            if (expression != null && expression.NodeType == NodeType.AwaitExpression)
            {
                // we force the await expression to be assigned to a temp variable
                var awaitExpression = (AwaitExpression)expression;
                awaitExpression.BaseExpression = VisitExpression(ref builder, awaitExpression.BaseExpression);

                var local = _F.DeclareTempLocal(_currentMethod, awaitExpression.ExpressionType);
                var replacement = _F.CreateAssignment(
                    awaitExpression.LexicalInfo,
                    _F.CreateLocalReference(local),
                    awaitExpression);
                if (builder == null)
                {
                    builder = new BoundSpillSequenceBuilder();
                }

                builder.AddLocal(local);
                builder.AddStatement(new ExpressionStatement(replacement));
                return _F.CreateLocalReference(local);
            }

            var e = Visit(expression);
            if (e == null || e.NodeType != SpillSequenceBuilder)
            {
                return e;
            }

            var newBuilder = (BoundSpillSequenceBuilder)e;
            if (builder == null)
            {
                builder = newBuilder.Update(null);
            }
            else
            {
                builder.Include(newBuilder);
            }

            return newBuilder.Value;
        }

        private static Expression UpdateExpression(BoundSpillSequenceBuilder builder, Expression expression)
        {
            if (builder == null)
            {
                return expression;
            }

            Debug.Assert(builder.Value == null);
            if (!builder.HasLocals && !builder.HasStatements)
            {
                return expression;
            }

            return builder.Update(expression);
        }

	    private static void UpdateConditionalStatement(ConditionalStatement stmt)
	    {
		    var builder = (BoundSpillSequenceBuilder) stmt.Condition;
		    Debug.Assert(stmt.ParentNode.NodeType == NodeType.Block);
			Debug.Assert(builder.Value.ExpressionType == My<TypeSystemServices>.Instance.BoolType);
			var spills = new Block(builder.GetStatements().ToArray());
			var statements = ((Block)stmt.ParentNode).Statements;
			statements.Insert(statements.IndexOf(stmt), spills);
		    stmt.Condition = builder.Value;
	    }

        private static Statement UpdateStatement(BoundSpillSequenceBuilder builder, Statement statement)
        {
            if (builder == null)
            {
                // statement doesn't contain any await
                Debug.Assert(statement != null);
                return statement;
            }

            Debug.Assert(builder.Value == null);
            if (statement != null)
            {
                builder.AddStatement(statement);
            }

            var result = new Block(builder.GetStatements().ToArray());

            return result;
        }

        private readonly object AWAIT_SPILL_MARKER = new object();

        private Expression Spill(
            BoundSpillSequenceBuilder builder,
            Expression expression,
            bool isRef = false,
            bool sideEffectsOnly = false)
        {
            Debug.Assert(builder != null);

            while (true)
            {
                switch (expression.NodeType)
                {
                    case NodeType.ListLiteralExpression:
                    case NodeType.ArrayLiteralExpression:
                        Debug.Assert(!isRef);
                        Debug.Assert(!sideEffectsOnly);
                        var arrayInitialization = (ListLiteralExpression)expression;
                        var newInitializers = VisitExpressionList(ref builder, arrayInitialization.Items, forceSpill: true);
                        arrayInitialization.Items = newInitializers;
                        return arrayInitialization;

                    case NodeType.HashLiteralExpression:
                        Debug.Assert(!isRef);
                        Debug.Assert(!sideEffectsOnly);
                        var hashInitialization = (HashLiteralExpression)expression;
                        var newInitializerPairs = VisitExpressionPairList(ref builder, hashInitialization.Items, forceSpill: true);
                        hashInitialization.Items = newInitializerPairs;
                        return hashInitialization;

                    case SpillSequenceBuilder:
                        var sequenceBuilder = (BoundSpillSequenceBuilder)expression;
                        builder.Include(sequenceBuilder);
                        expression = sequenceBuilder.Value;
                        continue;

                    case NodeType.SelfLiteralExpression:
                    case NodeType.SuperLiteralExpression:
                        if (isRef || !expression.ExpressionType.IsValueType)
                        {
                            return expression;
                        }
                        goto default;

                    case NodeType.ParameterDeclaration:
                        if (isRef)
                        {
                            return expression;
                        }
                        goto default;

                    case NodeType.ReferenceExpression:
                        var local = expression.Entity as InternalLocal;
                        if (local != null)
                        {
                            if (local.Local["SynthesizedKind"] == AWAIT_SPILL_MARKER || isRef)
                            {
                                return expression;
                            }
                        }
                        goto default;

                    case NodeType.MemberReferenceExpression:
                        if (expression.Entity.EntityType == EntityType.Field)
                        {
                            var field = (IField)expression.Entity;
                            if (field.IsInitOnly)
                            {
                                if (field.IsStatic) return expression;
                                if (!field.DeclaringType.IsValueType)
                                {
                                    // save the receiver; can get the field later.
                                    var target = Spill(builder,
                                        ((MemberReferenceExpression) expression).Target,
                                        isRef && !field.Type.IsValueType,
                                        sideEffectsOnly);
                                    return _F.CreateMemberReference(target, field);
                                }
                            }
                        }
                        goto default;

                    case NodeType.MethodInvocationExpression:
                        var mie = (MethodInvocationExpression) expression;
                        if (expression.Entity == BuiltinFunction.Eval)
                        {
                            builder.AddExpressions(mie.Arguments.Where(a => a != mie.Arguments.Last));
                            expression = mie.Arguments.Last;
                            continue;
                        }
                        if (isRef)
                        {
                            Debug.Assert(mie.ExpressionType.IsPointer);
                            CompilerContext.Current.Errors.Add(CompilerErrorFactory.UnsafeReturnInAsync(mie));
                        }
                        goto default;

                    case NodeType.BoolLiteralExpression:
                    case NodeType.CharLiteralExpression:
                    case NodeType.DoubleLiteralExpression:
                    case NodeType.IntegerLiteralExpression:
                    case NodeType.NullLiteralExpression:
                    case NodeType.RELiteralExpression:
                    case NodeType.StringLiteralExpression:
                    case NodeType.TypeofExpression:
                        return expression;

                    default:
                        if (expression.ExpressionType == _tss.VoidType || sideEffectsOnly)
                        {
                            builder.AddStatement(new ExpressionStatement(expression));
                            return null;
                        }
                        var replacement = _F.DeclareTempLocal(_currentMethod, expression.ExpressionType);

                        var assignToTemp = _F.CreateAssignment(
                            _F.CreateLocalReference(replacement),
                            expression);

                        builder.AddLocal(replacement);
                        builder.AddStatement(new ExpressionStatement(assignToTemp));
                        return _F.CreateLocalReference(replacement);
                }
            }
        }

        private ExpressionCollection VisitExpressionList(
            ref BoundSpillSequenceBuilder builder,
            ExpressionCollection args,
            IList<bool> refKinds = default(IList<bool>),
            bool forceSpill = false,
            bool sideEffectsOnly = false)
        {
            Visit(args);

            int lastSpill;
            if (forceSpill)
            {
                lastSpill = args.Count - 1;
            }
            else
            {
                lastSpill = -1;
                for (int i = args.Count - 1; i >= 0; i--)
                {
                    if (args[i].NodeType == SpillSequenceBuilder)
                    {
                        lastSpill = i;
                        break;
                    }
                }
            }

            if (lastSpill == -1)
            {
                return args;
            }

            if (builder == null)
            {
                builder = new BoundSpillSequenceBuilder();
            }

            for (int i = 0; i <= lastSpill; i++)
            {
                var refKind = refKinds != null && refKinds.Count > i && refKinds[i];
                var replacement = Spill(builder, args[i], refKind, sideEffectsOnly);

                Debug.Assert(sideEffectsOnly || replacement != null);
                if (!sideEffectsOnly)
                {
                    args[i] = replacement;
                }
            }

            return args;
        }

        private ExpressionPairCollection VisitExpressionPairList(
            ref BoundSpillSequenceBuilder builder,
            ExpressionPairCollection args,
            bool forceSpill = false,
            bool sideEffectsOnly = false)
        {
            var args1 = new ExpressionCollection();
            args1.AddRange(args.Select(p => p.First));
            var args2 = new ExpressionCollection();
            args2.AddRange(args.Select(p => p.Second));
            args1 = VisitExpressionList(ref builder, args1, null, forceSpill, sideEffectsOnly);
            args2 = VisitExpressionList(ref builder, args2, null, forceSpill, sideEffectsOnly);
            args.Clear();
            args.AddRange(args1.Zip(args2, (l, r) => new ExpressionPair(l.LexicalInfo, l, r)));
            return args;
        }

        private SliceCollection VisitSliceCollection(
            ref BoundSpillSequenceBuilder builder,
            SliceCollection args)
        {
	        foreach (var arg in args)
	        {
		        if (arg.Begin != null)
			        VisitExpression(ref builder, arg.Begin);
				if (arg.End != null)
					VisitExpression(ref builder, arg.End);
				if (arg.Step != null)
					VisitExpression(ref builder, arg.Step);
			}
            return args;
        }

        #region Statement Visitors

        public override void OnRaiseStatement(RaiseStatement node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Exception = VisitExpression(ref builder, node.Exception);
            ReplaceCurrentNode(UpdateStatement(builder, node));
        }

        public override void OnExpressionStatement(ExpressionStatement node)
        {
            BoundSpillSequenceBuilder builder = null;
            Expression expr;

            if (node.Expression.NodeType == NodeType.AwaitExpression)
            {
                // await expression with result discarded
                var awaitExpression = (AwaitExpression)node.Expression;
                awaitExpression.BaseExpression = VisitExpression(ref builder, awaitExpression.BaseExpression);
                expr = awaitExpression;
            }
			else if (node.Expression.NodeType == NodeType.MethodInvocationExpression && 
				((MethodInvocationExpression)node.Expression).Target.Entity == BuiltinFunction.Switch) 
			{
				OnSwitch((MethodInvocationExpression)node.Expression, ref builder);
				expr = node.Expression;
			}
            else
            {
                expr = VisitExpression(ref builder, node.Expression);
            }

            Debug.Assert(expr != null);
            Debug.Assert(builder == null || builder.Value == null);
            node.Expression = expr;
            ReplaceCurrentNode(UpdateStatement(builder, node));
        }

        public override void OnReturnStatement(ReturnStatement node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Expression = VisitExpression(ref builder, node.Expression);
            ReplaceCurrentNode(UpdateStatement(builder, node));
        }

	    public override void OnIfStatement(IfStatement node)
	    {
			base.OnIfStatement(node);
			if (node.Condition.NodeType == SpillSequenceBuilder)
			    UpdateConditionalStatement(node);
	    }

	    #endregion

        #region Expression Visitors

        public override void OnAwaitExpression(AwaitExpression node)
        {
            var builder = new BoundSpillSequenceBuilder();
            var replacement = VisitExpression(ref builder, node);
            ReplaceCurrentNode(builder.Update(replacement));
        }

        public override void OnSlicingExpression(SlicingExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            var target = VisitExpression(ref builder, node.Target);

            BoundSpillSequenceBuilder indicesBuilder = null;
            var indices = VisitSliceCollection(ref indicesBuilder, node.Indices);

            if (indicesBuilder != null)
            {
                // spill the array if there were await expressions in the indices
                if (builder == null)
                {
                    builder = new BoundSpillSequenceBuilder();
                }

                target = Spill(builder, target);
            }

            if (builder != null)
            {
                builder.Include(indicesBuilder);
                indicesBuilder = builder;
            }
            node.Target = target;
            node.Indices = indices;
            ReplaceCurrentNode(UpdateExpression(indicesBuilder, node));
        }

        public override void OnListLiteralExpression(ListLiteralExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Items = VisitExpressionList(ref builder, node.Items);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        public override void OnArrayLiteralExpression(ArrayLiteralExpression node)
        {
            OnListLiteralExpression(node);
        }

        public override void OnHashLiteralExpression(HashLiteralExpression node)
        {

            BoundSpillSequenceBuilder builder = null;
            node.Items = VisitExpressionPairList(ref builder, node.Items);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        public override void OnTryCastExpression(TryCastExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Target = VisitExpression(ref builder, node.Target);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        private void OnAssignment(BinaryExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            var right = VisitExpression(ref builder, node.Right);
            Expression left;
            if (builder == null || node.Left.Entity.EntityType == EntityType.Local)
            {
                left = VisitExpression(ref builder, node.Left);
            }
            else
            {
                // if the right-hand-side has await, spill the left
                var leftBuilder = new BoundSpillSequenceBuilder();
                left = VisitExpression(ref leftBuilder, node.Left);
                if (left.Entity.EntityType == EntityType.Local)
                {
                    left = Spill(leftBuilder, left, true);
                }

                leftBuilder.Include(builder);
                builder = leftBuilder;
            }
            node.Left = left;
            node.Right = right;
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        private void OnIsaOperator(BinaryExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Left = VisitExpression(ref builder, node.Left);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        public override void OnBinaryExpression(BinaryExpression node)
        {
            if (node.Operator == BinaryOperatorType.Assign)
            {
                OnAssignment(node);
                return;
            }
            if (node.Operator == BinaryOperatorType.TypeTest)
            {
                OnIsaOperator(node);
                return;
            }

            BoundSpillSequenceBuilder builder = null;
            var right = VisitExpression(ref builder, node.Right);
            Expression left;
            if (builder == null)
            {
                left = VisitExpression(ref builder, node.Left);
            }
            else
            {
                var leftBuilder = new BoundSpillSequenceBuilder();
                left = VisitExpression(ref leftBuilder, node.Left);
                left = Spill(leftBuilder, left);
                if (node.Operator == BinaryOperatorType.Or || node.Operator == BinaryOperatorType.And)
                {
                    var tmp = _F.DeclareTempLocal(_currentMethod, node.ExpressionType);
                    tmp.Local["SynthesizedKind"] = AWAIT_SPILL_MARKER;
                    leftBuilder.AddLocal(tmp);
                    leftBuilder.AddStatement(new ExpressionStatement(_F.CreateAssignment(_F.CreateLocalReference(tmp), left)));
                    var trueBlock = new Block();
                    trueBlock.Add(UpdateExpression(builder, _F.CreateAssignment(_F.CreateLocalReference(tmp), right)));
                    leftBuilder.AddStatement(
                        new IfStatement(left.LexicalInfo,
                            node.Operator == BinaryOperatorType.And ? 
                                _F.CreateLocalReference(tmp) : 
                                (Expression)_F.CreateNotExpression(_F.CreateLocalReference(tmp)),
                            trueBlock,
                            null));

                    ReplaceCurrentNode(UpdateExpression(leftBuilder, _F.CreateLocalReference(tmp)));
                    return;
                }
                // if the right-hand-side has await, spill the left
                leftBuilder.Include(builder);
                builder = leftBuilder;
            }

            node.Left = left;
            node.Right = right;
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        private void OnEval(MethodInvocationExpression node)
        {
            BoundSpillSequenceBuilder valueBuilder = null;
            var value = VisitExpression(ref valueBuilder, node.Arguments.Last);

            BoundSpillSequenceBuilder builder = null;
            var seCollection = new ExpressionCollection();
            seCollection.AddRange(node.Arguments.Where(a => a != node.Arguments.Last));
            var sideEffects = VisitExpressionList(ref builder, seCollection, forceSpill: valueBuilder != null, sideEffectsOnly: true);

            if (builder == null && valueBuilder == null)
            {
                node.Arguments = sideEffects;
                node.Arguments.Add(value);
                return;
            }

            if (builder == null)
            {
                builder = new BoundSpillSequenceBuilder();
            }

            builder.AddExpressions(sideEffects);
            builder.Include(valueBuilder);

            ReplaceCurrentNode(builder.Update(value));
        }

        public override void OnMethodInvocationExpression(MethodInvocationExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            var entity = node.Target.Entity;
	        if (entity.EntityType == EntityType.BuiltinFunction)
	        {
		        if (entity == BuiltinFunction.Eval)
		        {
			        OnEval(node);
		        }
		        else if (entity == BuiltinFunction.Switch)
		        {
					throw new ArgumentException("Should be unreachable: Await spiller on switch");
		        }
				else base.OnMethodInvocationExpression(node);
				return;
			}

            var method = (IMethod) entity;
            var refs = method.GetParameters().Select(p => p.IsByRef).ToList();
            node.Arguments = VisitExpressionList(ref builder, node.Arguments, refs);

            if (builder == null)
            {
                node.Target = VisitExpression(ref builder, node.Target);
            }
            else if (!method.IsStatic)
            {
                // spill the receiver if there were await expressions in the arguments
                var targetBuilder = new BoundSpillSequenceBuilder();

                var target = node.Target;
                var isRef = TargetSpillRefKind(target);

                node.Target = Spill(targetBuilder, VisitExpression(ref targetBuilder, target), isRef);
                targetBuilder.Include(builder);
                builder = targetBuilder;
            }

            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

		private void OnSwitch(MethodInvocationExpression node, ref BoundSpillSequenceBuilder builder)
		{
			node.Arguments = VisitExpressionList(ref builder, node.Arguments);
		}

	    private static bool TargetSpillRefKind(Expression target)
        {
            if (target.ExpressionType.IsValueType)
            {
                switch (target.NodeType)
                {
                    case NodeType.SlicingExpression:
                    case NodeType.SelfLiteralExpression:
                    case NodeType.SuperLiteralExpression:
                        return true;

                    case NodeType.ReferenceExpression:
                    case NodeType.MemberReferenceExpression:
                        switch (target.Entity.EntityType)
                        {
                            case EntityType.Field:
                            case EntityType.Parameter:
                            case EntityType.Local:
                                return true;
                            default:
                                return false;
                        }

                    case NodeType.UnaryExpression:
                        return ((UnaryExpression) target).Operator == UnaryOperatorType.Indirection;

                    case NodeType.MethodInvocationExpression:
                        return ((IMethod)target.Entity).Type.IsPointer;
                }
            }
            return false;
        }

        public override void OnConditionalExpression(ConditionalExpression node)
        {
            BoundSpillSequenceBuilder conditionBuilder = null;
            var condition = VisitExpression(ref conditionBuilder, node.Condition);

            BoundSpillSequenceBuilder trueBuilder = null;
            var trueValue = VisitExpression(ref trueBuilder, node.TrueValue);

            BoundSpillSequenceBuilder falseBuilder = null;
            var falseValue = VisitExpression(ref falseBuilder, node.FalseValue);

            if (trueBuilder == null && falseBuilder == null)
            {
                node.Condition = condition;
                node.TrueValue = trueValue;
                node.FalseValue = falseValue;
                ReplaceCurrentNode(UpdateExpression(conditionBuilder, node));
                return;
            }

            if (conditionBuilder == null) conditionBuilder = new BoundSpillSequenceBuilder();
            if (trueBuilder == null) trueBuilder = new BoundSpillSequenceBuilder();
            if (falseBuilder == null) falseBuilder = new BoundSpillSequenceBuilder();

            if (node.ExpressionType == _tss.VoidType)
            {
                conditionBuilder.AddStatement(
                    new IfStatement(
                        condition.LexicalInfo,
                        condition,
                        new Block(UpdateStatement(trueBuilder, new ExpressionStatement(trueValue))),
                        new Block(UpdateStatement(falseBuilder, new ExpressionStatement(falseValue)))));

                ReplaceCurrentNode(conditionBuilder.Update(_F.CreateDefaultInvocation(node.LexicalInfo, node.ExpressionType)));
            }
            else
            {
                var tmp = _F.DeclareTempLocal(_currentMethod, node.ExpressionType);
                tmp.Local["SynthesizedKind"] = AWAIT_SPILL_MARKER;

                conditionBuilder.AddLocal(tmp);
                var trueBlock = new Block(new ExpressionStatement(
                    UpdateExpression(trueBuilder, _F.CreateAssignment(_F.CreateLocalReference(tmp), trueValue))));
                var falseBlock = new Block(new ExpressionStatement(
                    UpdateExpression(falseBuilder, _F.CreateAssignment(_F.CreateLocalReference(tmp), falseValue))));
                conditionBuilder.AddStatement(new IfStatement(condition, trueBlock, falseBlock));

                ReplaceCurrentNode(conditionBuilder.Update(_F.CreateLocalReference(tmp)));
            }
        }

        public override void OnCastExpression(CastExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Target = VisitExpression(ref builder, node.Target);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        public override void OnMemberReferenceExpression(MemberReferenceExpression node)
        {
            if (node.Entity.EntityType == EntityType.Field || node.Entity.EntityType == EntityType.Method)
            {
                BoundSpillSequenceBuilder builder = null;
                node.Target = VisitExpression(ref builder, node.Target);
                ReplaceCurrentNode(UpdateExpression(builder, node));
                return;
            }
            base.OnMemberReferenceExpression(node);
        }

        public override void OnUnaryExpression(UnaryExpression node)
        {
            BoundSpillSequenceBuilder builder = null;
            node.Operand = VisitExpression(ref builder, node.Operand);
            ReplaceCurrentNode(UpdateExpression(builder, node));
        }

        #endregion
    }
}
