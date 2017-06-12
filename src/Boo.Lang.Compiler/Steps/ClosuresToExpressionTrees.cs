using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Generics;
using Expression = System.Linq.Expressions.Expression;

namespace Boo.Lang.Compiler.Steps
{
	public class ClosuresToExpressionTrees : AbstractTransformerCompilerStep
	{
		private static MethodInvocationExpression ExpressionFactory(LexicalInfo info, string name, params Ast.Expression[] args)
		{
			return new MethodInvocationExpression(
				info,
				new MemberReferenceExpression(
					info,
					new MemberReferenceExpression(
						new MemberReferenceExpression(
							new MemberReferenceExpression(new ReferenceExpression("System"), "Linq"),
							"Expressions"),
						"Expression"),
					name),
				args);
		}
		
		private static MethodInvocationExpression ExpressionFactory(string name, params Ast.Expression[] args)
		{
			return new MethodInvocationExpression(
				new MemberReferenceExpression(
					new MemberReferenceExpression(
						new MemberReferenceExpression(
							new MemberReferenceExpression(new ReferenceExpression("System"), "Linq"),
							"Expressions"),
						"Expression"),
					name),
				args);
		}
		
		private class BlockContext
		{
			public List<Declaration> Declarations = new List<Declaration>();
			
			public StatementModifier CurrentModifier;
			
			private TypeReference _returnType;
			
			private Dictionary<string, string> _args = new Dictionary<string, string>();
			
			public BlockContext(Ast.BlockExpression node)
			{
				_returnType = node.ReturnType;
				foreach (var arg in node.Parameters)
					_args.Add(arg.Name, null);
			}
			
			public bool HasArg(string name)
			{
				return _args.ContainsKey(name);
			}
			
			public bool UsesArg(string name)
			{
				return _args[name] != null;
			}
			
			public ReferenceExpression GetArg(string name)
			{
				var result = _args[name];
				if (result == null)
				{
					result = CompilerContext.Current.GetUniqueName("p", name);
					_args[name] = result;
				}
				return new ReferenceExpression(result);
			}
			
			public void AddDeclaration(Declaration node)
			{
				Declarations.Add(node);
			}
			
			public void ClearModifier()
			{
				CurrentModifier = null;
			}
			
			public ExpressionStatement Modify(Ast.Expression expr)
			{
				try {
					if (CurrentModifier == null || CurrentModifier.Type == StatementModifierType.None)
						return new ExpressionStatement(expr);
					MethodInvocationExpression result;
					switch (CurrentModifier.Type)
					{
						case StatementModifierType.If: result = ExpressionFactory(expr.LexicalInfo, "IfThen",
								CurrentModifier.Condition,
								expr);
							break;
						case StatementModifierType.Unless: result = ExpressionFactory(expr.LexicalInfo, "IfThen",
								new UnaryExpression(
									CurrentModifier.Condition.LexicalInfo,
									UnaryOperatorType.UnaryNegation,
									CurrentModifier.Condition),
								expr);
							break;
						case StatementModifierType.While: 
							CompilerContext.Current.Errors.Add(new CompilerError(CurrentModifier,
							                                      "While modifier not supported in converting lambdas to expression trees"));
							return new ExpressionStatement(expr);
						default:
							CompilerContext.Current.Errors.Add(new CompilerError(CurrentModifier,
							                                      "Unrecognized modifier while converting lambdas to expression trees"));
							return new ExpressionStatement(expr);
					}
					return new ExpressionStatement(result);
				} finally {
					CurrentModifier = null;
				}
			}
		}
		
		private int _processing;
		
		private bool Processing
		{
			get {return _processing > 0;}
		}
		
		private Stack<BlockContext> _blocks = new Stack<BlockContext>();
		
		private void DeleteClosure(InternalMethod method)
		{
			method.Method.DeclaringType.Members.Remove(method.Method);
		}
		
		override public void OnBlockExpression(Ast.BlockExpression node)
		{
			if (node.IsExpressionTree || Processing)
			{
				++ _processing;
				MethodInvocationExpression replacement = null;
				try {
					replacement = ConvertClosure(node);
					var owner = node.GetAncestor<Block>();
					ReplaceCurrentNode(replacement);

					var pmb = new ProcessMethodBodies();
					var method = node.GetAncestor<Method>();
					var cleanup = pmb.Initialize(this.Context, method);
					var args = (List<DeclarationStatement>)node["closureArgs"];
					foreach (var arg in args) {
						owner.Statements.Insert(0, arg);
						arg.Accept(pmb);
					}
					replacement.Accept(pmb);
					cleanup();
					DeleteClosure((InternalMethod)node.Entity);
				} finally {
					-- _processing;
				}
			}
			else Visit(node.Body);
		}
		
		private Ast.MethodInvocationExpression ExtractBlockExpr(Block block)
		{
			if (block.Statements.IsEmpty)
				return null;
			else return (MethodInvocationExpression)((ExpressionStatement)block.FirstStatement).Expression;
		}
		
		private MethodInvocationExpression MakeGenericLambda(Ast.BlockExpression node, MethodInvocationExpression bodyExpr, 
			List<ReferenceExpression> args, GenericTypeReference delegateType)
		{
				var inv = ExpressionFactory(node.LexicalInfo, "Lambda", bodyExpr);
				if (! (inv.Target is MemberReferenceExpression && ((MemberReferenceExpression)inv.Target).Name == "Lambda"))
					throw new Exception("Invalid codegen");
				inv.Arguments.AddRange(args);
				var gen = new GenericReferenceExpression(node.LexicalInfo);
				gen.Target = inv.Target;
				gen.GenericArguments.Add(delegateType);
				inv.Target = gen;
				return inv;
		}
		
		private MethodInvocationExpression ConvertClosure(Ast.BlockExpression node)
		{
			_blocks.Push(new BlockContext(node));
			try {
				Visit(node.Body);
				var args = new List<ReferenceExpression>();
				var declarations = new List<DeclarationStatement>();
				var delegateType = new GenericTypeReference(node.LexicalInfo, "System.Func");
				var parentBlock = node.GetAncestor<Ast.Block>();
				foreach (var param in node.Parameters)
				{
					if (! _blocks.Peek().UsesArg(param.Name))
						continue;
					
					if (param.IsByRef) {
						Context.Errors.Add(new CompilerError(param, "Unable to convert Ref params in lambdas to expression trees"));
						continue;
					}
					var argRef = _blocks.Peek().GetArg(param.Name);
					var arg = new DeclarationStatement(
						param.LexicalInfo,
						new Declaration(param.LexicalInfo, argRef.Name),
						ExpressionFactory(
							param.LexicalInfo,
							"Parameter",
							new TypeofExpression(param.LexicalInfo, param.Type)));
					declarations.Add(arg);
					args.Add(argRef);
					delegateType.GenericArguments.Add(param.Type);
				}
				delegateType.GenericArguments.Add(node.ReturnType);
				var bodyExpr = ExtractBlockExpr(node.Body);
				node["closureArgs"] = declarations;
				return MakeGenericLambda(node, bodyExpr, args, delegateType);
			} finally {
				_blocks.Pop();
			}
		}
		
		private Ast.Expression MemberReference(Ast.Expression target)
		{
			if (target.GetType() == typeof(ReferenceExpression))
			{
				var name = ((ReferenceExpression)target).Name;
				if (_blocks.Peek().HasArg(name))
					return _blocks.Peek().GetArg(name);
			}
			return ExpressionFactory(
				target.LexicalInfo,
				"Constant",
				target);
		}

		public override void OnMemberReferenceExpression(Ast.MemberReferenceExpression node)
		{
			if (Processing) {
				Visit(node.Target);
				switch (node.Entity.EntityType) {
					case EntityType.Method: break;
					case EntityType.Field:
					case EntityType.Property: 
						ReplaceCurrentNode(ExpressionFactory(
							node.LexicalInfo,
							"PropertyOrField",
							MemberReference(node.Target),
							new StringLiteralExpression(node.Name)));
						break;
					default:
						CompilerContext.Current.Errors.Add(new CompilerError(node,
						                                      "Unrecognized member reference type while converting lambdas to expression trees"));
						break;
				}
			}
			else base.OnMemberReferenceExpression(node);
		}

		public override void OnDeclaration(Ast.Declaration node)
		{
			if (Processing)
				_blocks.Peek().AddDeclaration(node);
		}
		
		public override void OnStatementModifier(Ast.StatementModifier node)
		{
			if (Processing)
				_blocks.Peek().CurrentModifier = node;
		}
		
		public override void OnGotoStatement(Ast.GotoStatement node)
		{
			if (Processing)
				Context.Errors.Add(new CompilerError(node, "Gotos are not supported in converting lambdas to expression trees"));
		}
		
		private IEnumerable<Ast.Expression> ExtractStatements(IEnumerable<Statement> statements)
		{
			foreach (var statement in statements)
				if (statement is ExpressionStatement)
					yield return ((ExpressionStatement)statement).Expression;
		}
		
		public override void OnBlock(Ast.Block node)
		{
			base.OnBlock(node);
			if (Processing) {
				var statements = node.Statements.ToArray();
				if (statements.Length != 1) {
					Context.Errors.Add(new CompilerError(node, "Blocks must contain only one expression in converting lambdas to expression trees"));
					return;
				}
				ReplaceCurrentNode((ExpressionStatement)statements[0]);
			}
		}
		
		public override void OnDeclarationStatement(Ast.DeclarationStatement node)
		{
			base.OnDeclarationStatement(node);
			if (Processing) {
				if (node.Initializer == null)
				{
					RemoveCurrentNode();
				} else {
					ReplaceCurrentNode(_blocks.Peek().Modify(node.Initializer));
				}
			}
		}
		
		private MethodInvocationExpression TryFinallyBlock(TryStatement node)
		{
			return ExpressionFactory(node.LexicalInfo, "TryFinally",
				ExtractBlockExpr(node.ProtectedBlock),
				ExtractBlockExpr(node.EnsureBlock));
		}
		
		private MethodInvocationExpression TryFaultBlock(TryStatement node)
		{
			return ExpressionFactory(node.LexicalInfo, "TryFault",
				ExtractBlockExpr(node.ProtectedBlock),
				ExtractBlockExpr(node.FailureBlock));
		}
		
		private MethodInvocationExpression NestedTryBlock(MethodInvocationExpression block, string outerName, Block outerBlock)
		{
			return ExpressionFactory(block.LexicalInfo, outerName, ExtractBlockExpr(outerBlock));
		}
		
		private MethodInvocationExpression TryFaultFinallyBlock(TryStatement node)
		{
			var fault = TryFaultBlock(node);
			return NestedTryBlock(fault, "TryFinally", node.EnsureBlock);
		}
		
		private MethodInvocationExpression ConvertHandler(ExceptionHandler handler)
		{
			bool untypedException = (handler.Flags & ExceptionHandlerFlags.Untyped) == ExceptionHandlerFlags.Untyped;
			bool anonymousException = (handler.Flags & ExceptionHandlerFlags.Anonymous) == ExceptionHandlerFlags.Anonymous;
			bool filterHandler = (handler.Flags & ExceptionHandlerFlags.Filter) == ExceptionHandlerFlags.Filter;
			var body = ExtractBlockExpr(handler.Block);
			Ast.Expression e;
			if (untypedException)
				e = new TypeofExpression(handler.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.ExceptionType));
			else e = new TypeofExpression(handler.LexicalInfo, handler.Declaration.Type);
			if (!anonymousException)
				e = ExpressionFactory(handler.Declaration.LexicalInfo, "Parameter",
						e,
						new ReferenceExpression(handler.Declaration.Name));
			var result = ExpressionFactory(handler.LexicalInfo, "Catch", e, body);
			if (filterHandler)
				result.Arguments.Add(handler.FilterCondition);
			return result;
		}
		
		private IEnumerable<MethodInvocationExpression> ExtractHandlers(ExceptionHandlerCollection coll)
		{
			foreach (ExceptionHandler handler in coll)
				yield return ConvertHandler(handler);
		}
		
		private MethodInvocationExpression TryExceptBlock(TryStatement node)
		{
			var result = ExpressionFactory(node.LexicalInfo, "TryCatch", ExtractBlockExpr(node.ProtectedBlock));
			result.Arguments.AddRange(ExtractHandlers(node.ExceptionHandlers));
			return result;
		}
		
		private MethodInvocationExpression TryExceptFinallyBlock(TryStatement node)
		{
			var result = ExpressionFactory(node.LexicalInfo, "TryCatchFinally",
				ExtractBlockExpr(node.ProtectedBlock),
				ExtractBlockExpr(node.EnsureBlock));
			result.Arguments.AddRange(ExtractHandlers(node.ExceptionHandlers));
			return result;
		}
		
		private MethodInvocationExpression TryExceptFaultBlock(TryStatement node)
		{
			var exc = TryExceptBlock(node);
			return NestedTryBlock(exc, "TryFault", node.FailureBlock);
		}
		
		private MethodInvocationExpression TryExceptFaultFinallyBlock(TryStatement node)
		{
			var fault = TryExceptFaultBlock(node);
			return NestedTryBlock(fault, "TryFinally", node.EnsureBlock);
		}
		
		public override void OnTryStatement(Ast.TryStatement node)
		{
			base.OnTryStatement(node);
			if (Processing) {
				MethodInvocationExpression result;
				if (node.ExceptionHandlers.IsEmpty)
				{
					if (node.FailureBlock == null)
					  result = TryFinallyBlock(node);
					else if (node.EnsureBlock == null)
						result = TryFaultBlock(node);
					else result = TryFaultFinallyBlock(node);
				}
				else if (node.FailureBlock == null && node.EnsureBlock == null)
					result = TryExceptBlock(node);
				else if (node.FailureBlock == null)
				  result = TryExceptFinallyBlock(node);
				else if (node.EnsureBlock == null)
					result = TryExceptFaultBlock(node);
				else result = TryExceptFaultFinallyBlock(node);
				ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, result));
			}
		}
		
		public override void OnExceptionHandler(Ast.ExceptionHandler node)
		{
			if (Processing) { //don't process the declaration node
				Ast.Expression currentFilterConditionValue = node.FilterCondition;
				if (null != currentFilterConditionValue)
				{
					Ast.Expression newValue = (Ast.Expression)VisitNode(currentFilterConditionValue);
					if (!object.ReferenceEquals(newValue, currentFilterConditionValue))
					{
						node.FilterCondition = newValue;
					}
				}
				
				Block currentBlockValue = node.Block;
				if (null != currentBlockValue)
				{			
					Block newValue = (Block)VisitNode(currentBlockValue);
					if (!object.ReferenceEquals(newValue, currentBlockValue))
					{
						node.Block = newValue;
					}
				}
			}
			else base.OnExceptionHandler(node);
		}
		
		public override void OnIfStatement(Ast.IfStatement node)
		{
			base.OnIfStatement(node);
			if (Processing) {
				MethodInvocationExpression result = ExpressionFactory(node.LexicalInfo, "IfThen",
					node.Condition,
					ExtractBlockExpr(node.TrueBlock));
				if (node.FalseBlock != null)
				{
					result.Arguments.Add(ExtractBlockExpr(node.FalseBlock));
					((ReferenceExpression)result.Target).Name = "IfThenElse";
				}
				ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, result));
			}
		}
		
		public override void OnUnlessStatement(Ast.UnlessStatement node)
		{
			base.OnUnlessStatement(node);
			if (Processing) {
				MethodInvocationExpression result = ExpressionFactory(node.LexicalInfo, "IfThen",
					ExpressionFactory("Not", node.Condition),
					ExtractBlockExpr(node.Block));
				ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, result));
			}
		}
		
		public override void OnForStatement(Ast.ForStatement node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Loops are not supported in converting lambdas to expression trees"));
			else base.OnForStatement(node);
		}
		
		public override void OnWhileStatement(Ast.WhileStatement node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Loops are not supported in converting lambdas to expression trees"));
			else base.OnWhileStatement(node);
		}
		
		public override void OnReturnStatement(Ast.ReturnStatement node)
		{
			base.OnReturnStatement(node);
			if (Processing) {
				ReplaceCurrentNode(new ExpressionStatement(node.LexicalInfo, node.Expression));
			}
		}
		
		public override void OnYieldStatement(Ast.YieldStatement node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Yield statements are not supported in converting lambdas to expression trees"));
			else base.OnYieldStatement(node);
		}
			
		public override void OnRaiseStatement(Ast.RaiseStatement node)
		{
			base.OnRaiseStatement(node);
			if (Processing) {
				var result = ExpressionFactory(node.LexicalInfo, "Throw", node.Exception);
				ReplaceCurrentNode(_blocks.Peek().Modify(result));
			}
		}
		
		public override void OnUnpackStatement(Ast.UnpackStatement node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Unpack statements are not supported in converting lambdas to expression trees"));
			else base.OnUnpackStatement(node);
		}
		
		public override void OnExpressionStatement(Ast.ExpressionStatement node)
		{
			base.OnExpressionStatement(node);
			if (Processing) {
				var expr = node.Expression;
				ReplaceCurrentNode(_blocks.Peek().Modify(expr));
			}
		}
		
		public override void OnMethodInvocationExpression(Ast.MethodInvocationExpression node)
		{
			if (Processing) {
				var method = new MemberReferenceExpression(node.Target, "Method");
				var result = ExpressionFactory(node.LexicalInfo, "Call", method);
				Visit(node.Arguments);
				if (!node.NamedArguments.IsEmpty)
					CompilerContext.Current.Errors.Add(
						new CompilerError(node, "Named arguments are not supported in converting lambdas to expression trees"));
				result.Arguments.AddRange(node.Arguments);
				if (! ((IMember)node.Target.Entity).IsStatic)
					result.Arguments.Insert(0, Visit(node.Target));
				ReplaceCurrentNode(result);
			}
			else base.OnMethodInvocationExpression(node);
		}
		
		public override void OnUnaryExpression(Ast.UnaryExpression node)
		{
			base.OnUnaryExpression(node);
			if (Processing) {
				string methodName = null;
				switch (node.Operator) {
					case UnaryOperatorType.UnaryNegation: methodName = "Negate"; break;
					case UnaryOperatorType.Increment: methodName = "PreIncrementAssign"; break;
					case UnaryOperatorType.Decrement: methodName = "PreDecrementAssign"; break;
					case UnaryOperatorType.PostIncrement: methodName = "PostIncrementAssign"; break;
					case UnaryOperatorType.PostDecrement: methodName = "PostDecrementAssign"; break;
					case UnaryOperatorType.LogicalNot: methodName = "Not"; break;
					case UnaryOperatorType.OnesComplement: methodName = "OnesComplement"; break;
				default:
					Context.Errors.Add(new CompilerError(node,
						string.Format("Unary operator {0} is not supported in converting lambdas to expression trees", node.Operator.ToString())));
					return;
				}
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, methodName, node.Operand));
			}
		}
		
		public override void OnBinaryExpression(Ast.BinaryExpression node)
		{
			base.OnBinaryExpression(node);
			if (Processing) {
				string methodName = null;
				bool IsChecked = Context.Parameters.Checked;
				switch (node.Operator) {
					case BinaryOperatorType.Addition: methodName = IsChecked ? "AddChecked" : "Add"; break;
					case BinaryOperatorType.Subtraction: methodName = IsChecked ? "SubtractChecked" : "Subtract"; break;
					case BinaryOperatorType.Multiply: methodName = IsChecked ? "MultiplyChecked" : "Multiply"; break;
					case BinaryOperatorType.Division: methodName = IsChecked ? "DivideChecked" : "Divide"; break;
					case BinaryOperatorType.Modulus: methodName = "Modulo"; break;
					case BinaryOperatorType.Exponentiation: methodName = "Power"; break;
					case BinaryOperatorType.LessThan: methodName = "LessThan"; break;
					case BinaryOperatorType.LessThanOrEqual: methodName = "LessThanOrEqual"; break;
					case BinaryOperatorType.GreaterThan: methodName = "GreaterThan"; break;
					case BinaryOperatorType.GreaterThanOrEqual: methodName = "GreaterThanOrEqual"; break;
					case BinaryOperatorType.Equality: methodName = "Equal"; break;
					case BinaryOperatorType.Inequality: methodName = "NotEqual"; break;
					case BinaryOperatorType.Assign: methodName = "Assign"; break;
					case BinaryOperatorType.InPlaceAddition: methodName = IsChecked ? "AddAssignChecked" : "AddAssign"; break;
					case BinaryOperatorType.InPlaceSubtraction: methodName = IsChecked ? "SubtractAssignChecked" : "SubtractAssign"; break;
					case BinaryOperatorType.InPlaceMultiply: methodName = IsChecked ? "MultiplyAssignChecked" : "MultiplyAssign"; break;
					case BinaryOperatorType.InPlaceDivision: methodName = "DivideAssign"; break;
					case BinaryOperatorType.InPlaceModulus: methodName = "ModuloAssign"; break;
					case BinaryOperatorType.InPlaceBitwiseAnd: methodName = "AndAssign"; break;
					case BinaryOperatorType.InPlaceBitwiseOr: methodName = "OrAssign"; break;
					case BinaryOperatorType.ReferenceEquality: methodName = "EeferenceEqual"; break;
					case BinaryOperatorType.ReferenceInequality: methodName = "EeferenceNotEqual"; break;
					case BinaryOperatorType.TypeTest: methodName = "TypeIs"; break;
					case BinaryOperatorType.Or:
					case BinaryOperatorType.BitwiseOr: methodName = "Or"; break;
					case BinaryOperatorType.And:
					case BinaryOperatorType.BitwiseAnd: methodName = "And"; break;
					case BinaryOperatorType.ExclusiveOr: methodName = "ExclusiveOr"; break;
					case BinaryOperatorType.InPlaceExclusiveOr: methodName = "ExclusiveOrAssign"; break;
					case BinaryOperatorType.ShiftLeft: methodName = "LeftShift"; break;
					case BinaryOperatorType.InPlaceShiftLeft: methodName = "LeftShiftAssign"; break;
					case BinaryOperatorType.ShiftRight: methodName = "RightShift"; break;
					case BinaryOperatorType.InPlaceShiftRight: methodName = "RightShiftAssign"; break;
				default:
					Context.Errors.Add(new CompilerError(node,
						string.Format("Binary operator {0} is not supported in converting lambdas to expression trees", node.Operator.ToString())));
					return;
				}
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, methodName, node.Left, node.Right));
			}
		}
		
		public override void OnConditionalExpression(Ast.ConditionalExpression node)
		{
			base.OnConditionalExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "IfThenElse", node.Condition, node.TrueValue, node.FalseValue));
			}
		}
		
		public override void OnStringLiteralExpression(Ast.StringLiteralExpression node)
		{
			base.OnStringLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.StringType))));
			}
		}
		
		public override void OnCharLiteralExpression(Ast.CharLiteralExpression node)
		{
			base.OnCharLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.CharType))));
			}
		}
		
		public override void OnTimeSpanLiteralExpression(Ast.TimeSpanLiteralExpression node)
		{
			base.OnTimeSpanLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.TimeSpanType))));
			}
		}
		
		public override void OnIntegerLiteralExpression(Ast.IntegerLiteralExpression node)
		{
			base.OnIntegerLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.IntType))));
			}
		}
		
		public override void OnDoubleLiteralExpression(Ast.DoubleLiteralExpression node)
		{
			base.OnDoubleLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.DoubleType))));
			}
		}
		
		public override void OnNullLiteralExpression(Ast.NullLiteralExpression node)
		{
			base.OnNullLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant", node.CleanClone()));
			}
		}

		public override void OnSelfLiteralExpression(Ast.SelfLiteralExpression node)
		{
			base.OnSelfLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant", node.CleanClone()));
			}
		}
		
		public override void OnSuperLiteralExpression(Ast.SuperLiteralExpression node)
		{
			base.OnSuperLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant", node.CleanClone()));
			}
		}
		
		public override void OnBoolLiteralExpression(Ast.BoolLiteralExpression node)
		{
			base.OnBoolLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.BoolType))));
			}
		}
		
		public override void OnRELiteralExpression(Ast.RELiteralExpression node)
		{
			base.OnRELiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, CodeBuilder.CreateTypeReference(TypeSystemServices.RegexType))));
			}
		}
		
		public override void OnSpliceExpression(Ast.SpliceExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Splice expressions are not supported in converting lambdas to expression trees"));
			else base.OnSpliceExpression(node);
		}
		
		public override void OnSpliceTypeReference(Ast.SpliceTypeReference node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Splice expressions are not supported in converting lambdas to expression trees"));
			else base.OnSpliceTypeReference(node);
		}
		
		public override void OnSpliceMemberReferenceExpression(Ast.SpliceMemberReferenceExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Splice expressions are not supported in converting lambdas to expression trees"));
			else base.OnSpliceMemberReferenceExpression(node);
		}
		
		public override void OnExpressionInterpolationExpression(Ast.ExpressionInterpolationExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Splice expressions are not supported in converting lambdas to expression trees"));
			else base.OnExpressionInterpolationExpression(node);
		}
		
		public override void OnHashLiteralExpression(Ast.HashLiteralExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Hash literal expressions are not supported in converting lambdas to expression trees"));
			else base.OnHashLiteralExpression(node);
		}
		
		public override void OnListLiteralExpression(Ast.ListLiteralExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "List literal expressions are not supported in converting lambdas to expression trees"));
			else base.OnListLiteralExpression(node);
		}
		
		public override void OnCollectionInitializationExpression(Ast.CollectionInitializationExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Initializers are not supported in converting lambdas to expression trees"));
			else base.OnCollectionInitializationExpression(node);
		}
		
		public override void OnArrayLiteralExpression(Ast.ArrayLiteralExpression node)
		{
			base.OnArrayLiteralExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "Constant",
					node.CleanClone(),
					new TypeofExpression(node.LexicalInfo, node.Type)));
			}
		}
		
		public override void OnGeneratorExpression(Ast.GeneratorExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Generator expressions are not supported in converting lambdas to expression trees"));
			else base.OnGeneratorExpression(node);
		}
		
		public override void OnExtendedGeneratorExpression(Ast.ExtendedGeneratorExpression node)
		{
			if (Processing)
				CompilerContext.Current.Errors.Add(
					new CompilerError(node, "Generator expressions are not supported in converting lambdas to expression trees"));
			else base.OnExtendedGeneratorExpression(node);
		}
		
		private IEnumerable<MethodInvocationExpression> Slices(SliceCollection values)
		{
			foreach (var slice in values)
			{
				if (slice.Begin != null && slice.End == null && slice.Step == null)
					yield return (MethodInvocationExpression)Visit(slice.Begin);
				else CompilerContext.Current.Errors.Add(
					new CompilerError(slice, "Slice expressions are not supported in converting lambdas to expression trees"));
			}
		}
		
		public override void OnSlicingExpression(Ast.SlicingExpression node)
		{
			base.OnSlicingExpression(node);
			if (Processing) {
				var result = ExpressionFactory(node.LexicalInfo, "ArrayAccess", node.Target);
				result.Arguments.AddRange(Slices(node.Indices));
				ReplaceCurrentNode(result);
			}
		}
		
		public override void OnTryCastExpression(Ast.TryCastExpression node)
		{
			base.OnTryCastExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(node.LexicalInfo, "TypeAs",
					node.Target,
					new TypeofExpression(node.Type.LexicalInfo, node.Type)));
			}
		}
		
		public override void OnCastExpression(Ast.CastExpression node)
		{
			base.OnCastExpression(node);
			if (Processing) {
				ReplaceCurrentNode(ExpressionFactory(
					node.LexicalInfo,
					Context.Parameters.Checked ? "ConvertChecked" : "Convert",
					node.Target,
					new TypeofExpression(node.Type.LexicalInfo, node.Type)));
			}
		}
	}
}