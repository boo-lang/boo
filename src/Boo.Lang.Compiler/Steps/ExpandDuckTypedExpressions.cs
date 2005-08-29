using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// </summary>
	public class ExpandDuckTypedExpressions : AbstractTransformerCompilerStep
	{
		protected IType _runtimeServices;
		protected IMethod RuntimeServices_Invoke;
		protected IMethod RuntimeServices_InvokeCallable;
		protected IMethod RuntimeServices_InvokeBinaryOperator;
		protected IMethod RuntimeServices_InvokeUnaryOperator;
		protected IMethod RuntimeServices_SetProperty;
		protected IMethod RuntimeServices_GetProperty;
		protected IMethod RuntimeServices_GetSlice;
		
		public ExpandDuckTypedExpressions()
		{
		}

		public override void Initialize(CompilerContext context)
		{
			base.Initialize(context);
			_runtimeServices = TypeSystemServices.Map(typeof(Boo.Lang.Runtime.RuntimeServices));
			RuntimeServices_Invoke = ResolveMethod(_runtimeServices, "Invoke");
			RuntimeServices_InvokeCallable = ResolveMethod(_runtimeServices, "InvokeCallable");
			RuntimeServices_InvokeBinaryOperator = ResolveMethod(_runtimeServices, "InvokeBinaryOperator");
			RuntimeServices_InvokeUnaryOperator = ResolveMethod(_runtimeServices, "InvokeUnaryOperator");
			RuntimeServices_SetProperty = ResolveMethod(_runtimeServices, "SetProperty");
			RuntimeServices_GetProperty = ResolveMethod(_runtimeServices, "GetProperty");
			RuntimeServices_GetSlice = ResolveMethod(_runtimeServices, "GetSlice");
		}

		private IMethod ResolveMethod(IType type, string name)
		{
			return NameResolutionService.ResolveMethod(type, name);
		}

		public override void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (TypeSystemServices.IsQuackBuiltin(node.Target))
			{
				Visit(node.Arguments);
				Visit(node.NamedArguments);
				ProcessQuackInvocation(node);
				return;
			}

			base.OnMethodInvocationExpression(node);
			if (!IsDuckTyped(node.Target)) return;
			
			MethodInvocationExpression invoke = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_InvokeCallable,
				node.Target,
				CodeBuilder.CreateObjectArray(node.Arguments));
			Replace(invoke);
		}
		
		override public void LeaveSlicingExpression(SlicingExpression node)
		{
			if (!IsDuckTyped(node.Target)) return;

			// todo
			// a[foo]
			// RuntimeServices.GetSlice(a, "", (foo,))
			ArrayLiteralExpression args = new ArrayLiteralExpression();
			foreach (Slice index in node.Indices)
			{
				if (AstUtil.IsComplexSlice(index))
				{
					throw CompilerErrorFactory.NotImplemented(index, "complex slice for duck");
				}
				args.Items.Add(index.Begin);
			}
			BindExpressionType(args, TypeSystemServices.ObjectArrayType);
			
			Expression target = node.Target;
			string memberName = "";
			
			if (NodeType.MemberReferenceExpression == target.NodeType)
			{
				MemberReferenceExpression mre = ((MemberReferenceExpression)target);
				target = mre.Target;
				memberName = mre.Name;
			}
			
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_GetSlice,
				target,
				CodeBuilder.CreateStringLiteral(memberName),
				args);
			
			Replace(mie);
		}
		
		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (IsDuckTyped(node.Operand) &&
				node.Operator == UnaryOperatorType.UnaryNegation)
			{
				MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
					RuntimeServices_InvokeUnaryOperator,
					CodeBuilder.CreateStringLiteral(
					AstUtil.GetMethodNameForOperator(node.Operator)),
					node.Operand);
				
				Replace(mie);
			}
		}
		
		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (BinaryOperatorType.Assign == node.Operator)
			{
				if (TypeSystemServices.IsQuackBuiltin(node.Left))
				{
					ProcessQuackPropertySet(node);
				}
				return;
			}

			if (!AstUtil.IsOverloadableOperator(node.Operator)) return;
			if (!IsDuckTyped(node.Left) && !IsDuckTyped(node.Right)) return;

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_InvokeBinaryOperator,
				CodeBuilder.CreateStringLiteral(
				AstUtil.GetMethodNameForOperator(node.Operator)),
				node.Left, node.Right);
			Replace(mie);
		}

		override public void LeaveMemberReferenceExpression(MemberReferenceExpression node)
		{
			if (!TypeSystemServices.IsQuackBuiltin(node)) return;
			if (AstUtil.IsLhsOfAssignment(node)
				|| AstUtil.IsTargetOfSlicing(node)) return;

			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_GetProperty,
				node.Target,
				CodeBuilder.CreateStringLiteral(node.Name));

			Replace(mie);
		}
		
		void ProcessQuackPropertySet(BinaryExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Left;
			MethodInvocationExpression mie = CodeBuilder.CreateMethodInvocation(
				RuntimeServices_SetProperty,
				target.Target,
				CodeBuilder.CreateStringLiteral(target.Name),
				node.Right);
			Replace(mie);
		}
		
		void ProcessQuackInvocation(MethodInvocationExpression node)
		{
			MemberReferenceExpression target = (MemberReferenceExpression)node.Target;
			node.Target = CodeBuilder.CreateMemberReference(
				CodeBuilder.CreateReference(node.LexicalInfo, _runtimeServices),
				RuntimeServices_Invoke);
			
			Expression args = CodeBuilder.CreateObjectArray(node.Arguments);
			node.Arguments.Clear();
			node.Arguments.Add(target.Target);
			node.Arguments.Add(CodeBuilder.CreateStringLiteral(target.Name));
			node.Arguments.Add(args);
		}

		bool IsDuckTyped(Expression expression)
		{
			IType type = expression.ExpressionType;
			return null != type && TypeSystemServices.IsDuckType(type);
		}

		private void BindDuck(Expression node)
		{
			BindExpressionType(node, TypeSystemServices.DuckType);
		}

		void Replace(Expression node)
		{
			BindDuck(node);
			ReplaceCurrentNode(node);
		}
	}
}
