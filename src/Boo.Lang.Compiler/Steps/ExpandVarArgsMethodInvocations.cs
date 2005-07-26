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
			ICallableType callable = node.Target.ExpressionType as ICallableType;
			if (callable == null) return;

			CallableSignature signature = callable.GetSignature();
			if (!signature.AcceptVarArgs) return;

			IParameter[] parameters = signature.Parameters;
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
