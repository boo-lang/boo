#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
			if (IsVarArgsInvocation(node, entityWithParameters))
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

		private static bool IsVarArgsInvocation(MethodInvocationExpression node, IEntityWithParameters entityWithParameters)
		{
			return entityWithParameters.AcceptVarArgs && !AstUtil.InvocationEndsWithExplodeExpression(node);
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
			var explodeExpression = array as UnaryExpression;
			if (explodeExpression != null)
				ReifyExplodeExpression(expectedArrayType, explodeExpression);
			else
				ReifyArrayLiteralExpression(expectedArrayType, (ArrayLiteralExpression)array);
			BindExpressionType(array, expectedArrayType);
		}

		private void ReifyExplodeExpression(IArrayType expectedArrayType, UnaryExpression explodeExpression)
		{
			if (explodeExpression.Operator != UnaryOperatorType.Explode)
				throw new InvalidOperationException();
			ReifyArrayLiteralType(expectedArrayType, explodeExpression.Operand);
		}

		private void ReifyArrayLiteralExpression(IArrayType expectedArrayType, ArrayLiteralExpression arrayLiteralExpression)
		{
			arrayLiteralExpression.Type = (ArrayTypeReference)CodeBuilder.CreateTypeReference(expectedArrayType);
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
