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
			
			if (node.Arguments.Count > 0 &&
				AstUtil.IsExplodeExpression(node.Arguments[-1]))
			{
				// explode the arguments
				node.Arguments.ReplaceAt(-1, ((UnaryExpression)node.Arguments[-1]).Operand);
				return;
			}

			IParameter[] parameters = signature.Parameters;
			int lenMinusOne = parameters.Length-1;
			IType varArgType = parameters[lenMinusOne].Type;

			ExpressionCollection varArgs = node.Arguments.PopRange(lenMinusOne);
			node.Arguments.Add(CodeBuilder.CreateArray(varArgType, varArgs));
		}
	}
}
