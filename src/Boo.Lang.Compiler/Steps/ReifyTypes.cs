using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Services;

namespace Boo.Lang.Compiler.Steps
{
	public class ReifyTypes : AbstractVisitorCompilerStep, ITypeMemberReifier
	{
		private IMethod _currentMethod;

		public override void Run()
		{
			if (Errors.Count > 0)
				return;

			Visit(CompileUnit);
		}

		public override void LeaveBinaryExpression(BinaryExpression node)
		{
			TryToReify(node.Right, GetExpressionType(node.Left));
		}

		public override void LeaveCastExpression(CastExpression node)
		{
			TryToReify(node.Target, GetExpressionType(node));
		}

		public override void LeaveTryCastExpression(TryCastExpression node)
		{
			TryToReify(node.Target, GetExpressionType(node));
		}

		public override bool EnterMethod(Method node)
		{
			_currentMethod = GetEntity(node);
			return true;
		}

		public override void OnBlockExpression(BlockExpression node)
		{	
		}

		public override void LeaveReturnStatement(ReturnStatement node)
		{
			if (node.Expression == null)
				return;

			TryToReify(node.Expression, _currentMethod.ReturnType);
		}

		public override void LeaveYieldStatement(YieldStatement node)
		{
			if (node.Expression == null)
				return;

			TryToReify(node.Expression, GeneratorItemTypeFrom(_currentMethod.ReturnType) ?? TypeSystemServices.ObjectArrayType);
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
					TryToReify(node.Arguments[i], parameters[i].Type);

				var varArgArrayType = parameters[lastParamIndex].Type.ElementType;
				for (int i=lastParamIndex; i < node.Arguments.Count; ++i)
					TryToReify(node.Arguments[i], varArgArrayType);
			}
			else
				for (int i = 0; i < parameters.Length; i++)
					TryToReify(node.Arguments[i], parameters[i].Type);

		}

		private void TryToReify(Expression candidate, IType expectedType)
		{
			if (IsEmptyArrayLiteral(candidate))
				ReifyArrayLiteralType(ArrayTypeFor(expectedType), candidate);
			else if (candidate.NodeType == NodeType.IntegerLiteralExpression && TypeSystemServices.IsIntegerNumber(expectedType))
				BindExpressionType(candidate, expectedType);
		}

		private IArrayType ArrayTypeFor(IType expectedType)
		{
			var arrayType = expectedType as IArrayType;
			if (arrayType != null)
				return arrayType;

			IType generatorItemType = GeneratorItemTypeFrom(expectedType);
			if (generatorItemType != null)
				return generatorItemType.MakeArrayType(1);

			return TypeSystemServices.ObjectArrayType;
		}

		private IType GeneratorItemTypeFrom(IType expectedType)
		{
			return TypeSystemServices.IsGenericGeneratorReturnType(expectedType) ? expectedType.ConstructedInfo.GenericArguments[0] : null;
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

		public TypeMember Reify(TypeMember member)
		{
			Visit(member);
			return member;
		}
	}
}
