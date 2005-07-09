using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class ExpandVarArgsMethodInvocations : AbstractVisitorCompilerStep
	{
		override public void Run()
		{
			if (0 == Errors.Count)
			{
				Visit(CompileUnit);
			}
		}

		public override void LeaveMethodInvocationExpression(Boo.Lang.Compiler.Ast.MethodInvocationExpression node)
		{
			IMethod method = node.Target.Entity as IMethod;
			if (method == null || !method.AcceptVarArgs) return;

			IParameter[] parameters = method.GetParameters();
			int lenMinusOne = parameters.Length-1;
			IType varArgType = parameters[lenMinusOne].Type;

			/*
			if (node.Arguments.Count == parameters.Length
				&& varArgType == node.Arguments[-1].ExpressionType)
			{
				return;
			}*/

			ExpressionCollection varArgs = node.Arguments.PopRange(lenMinusOne);
			node.Arguments.Add(CodeBuilder.CreateArray(varArgType, varArgs));
		}
	}
}
