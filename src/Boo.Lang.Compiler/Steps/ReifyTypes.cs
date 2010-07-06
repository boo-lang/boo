using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.Steps
{
	public class ReifyTypes : AbstractVisitorCompilerStep, ITypeMemberReifier
	{
		public override void Run()
		{
			if (Errors.Count > 0)
				return;

			Visit(CompileUnit);
		}

		public override void LeaveBinaryExpression(BinaryExpression node)
		{
			TryToReifyEmptyArrayLiteral(node.Right, GetExpressionType(node.Left));
		}

		public override void LeaveCastExpression(CastExpression node)
		{
			TryToReifyEmptyArrayLiteral(node.Target, GetExpressionType(node));
		}

		public override void LeaveTryCastExpression(TryCastExpression node)
		{
			TryToReifyEmptyArrayLiteral(node.Target, GetExpressionType(node));
		}

		public override void LeaveMethodInvocationExpression(MethodInvocationExpression node)
		{
			var entityWithParameters = node.Target.Entity as IEntityWithParameters;
			if (entityWithParameters == null)
				return;

			IParameter[] parameters = entityWithParameters.GetParameters();
			if (entityWithParameters.AcceptVarArgs)
			{
				var lastParamIndex = parameters.Length - 1;
				for (int i=0; i < lastParamIndex; ++i)
					TryToReifyEmptyArrayLiteral(node.Arguments[i], parameters[i].Type);

				var varArgArrayType = parameters[lastParamIndex].Type.ElementType;
				for (int i=lastParamIndex; i < node.Arguments.Count; ++i)
					TryToReifyEmptyArrayLiteral(node.Arguments[i], varArgArrayType);
			}
			else
				for (int i = 0; i < parameters.Length; i++)
					TryToReifyEmptyArrayLiteral(node.Arguments[i], parameters[i].Type);

		}

		private void TryToReifyEmptyArrayLiteral(Expression candidateArray, IType expectedType)
		{
			if (!IsEmptyArrayLiteral(candidateArray))
				return;
			ReifyArrayLiteralType((IArrayType)expectedType, candidateArray);
		}

		private void ReifyArrayLiteralType(IArrayType expectedArrayType, Expression array)
		{
			((ArrayLiteralExpression)array).Type = (ArrayTypeReference)CodeBuilder.CreateTypeReference(expectedArrayType);
			BindExpressionType(array, expectedArrayType);
		}

		private static bool IsEmptyArrayLiteral(Expression e)
		{	
			return e.ExpressionType == EmptyArrayType.Default;
		}

		public void Reify(TypeMember member)
		{
			member.Accept(this);
		}
	}
}
