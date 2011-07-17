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
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.Util;

namespace Boo.Lang.Compiler.Steps
{
	public class InjectImplicitBooleanConversions : AbstractNamespaceSensitiveVisitorCompilerStep
	{
		private IMethod _String_IsNullOrEmpty;
		private Method _currentMethod;

		public override void Dispose()
		{
			_String_IsNullOrEmpty = null;
			_currentMethod = null;
			base.Dispose();
		}

		public override void OnMethod(Method node)
		{
			_currentMethod = node;
			Visit(node.Body);
		}

		public override void OnConstructor(Constructor node)
		{
			OnMethod(node);
		}

		override public void LeaveUnlessStatement(UnlessStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveIfStatement(IfStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveConditionalExpression(ConditionalExpression node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		override public void LeaveWhileStatement(WhileStatement node)
		{
			node.Condition = AssertBoolContext(node.Condition);
		}

		public override void LeaveUnaryExpression(UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.LogicalNot:
					node.Operand = AssertBoolContext(node.Operand);
					break;
			}
		}

		public override void LeaveBinaryExpression(BinaryExpression node)
		{
			switch (node.Operator)
			{
				case BinaryOperatorType.And:
				case BinaryOperatorType.Or:
					BindLogicalOperator(node);
					break;
			}
		}

		void BindLogicalOperator(BinaryExpression node)
		{
			if (IsLogicalCondition(node))
				BindLogicalOperatorCondition(node);
			else
				BindLogicalOperatorExpression(node);
		}

		public static bool IsLogicalCondition(Expression node)
		{
			Node condition = node;
			while (IsLogicalExpression(condition.ParentNode))
				condition = condition.ParentNode;
			return IsConditionOfConditionalStatement(condition);
		}

		private static bool IsConditionOfConditionalStatement(Node condition)
		{
			var conditionalStatement = condition.ParentNode as ConditionalStatement;
			return conditionalStatement != null && conditionalStatement.Condition == condition;
		}

		static bool IsLogicalExpression(Node node)
		{
			switch (node.NodeType)
			{
				case NodeType.BinaryExpression:
					return AstUtil.GetBinaryOperatorKind((BinaryExpression)node) == BinaryOperatorKind.Logical;
				case NodeType.UnaryExpression:
					return ((UnaryExpression) node).Operator == UnaryOperatorType.LogicalNot;
			}
			return false;
		}

		private void BindLogicalOperatorExpression(BinaryExpression node)
		{
			var condition = AssertBoolContext(node.Left);
			if (condition != node.Left)
			{
				// implicit conversion, original value has to be preserved
				// a and b => (b if op_Implicit(a) else a)
				// a or b => (a if op_Implicit(a) else b)
				var local = DeclareTempLocal(GetExpressionType(node.Left));
				var a = CodeBuilder.CreateReference(local);
				var b = node.Right;
				var e = node.Operator == BinaryOperatorType.And
							? new ConditionalExpression(node.LexicalInfo) { Condition = condition, TrueValue = b, FalseValue = a }
							: new ConditionalExpression(node.LexicalInfo) { Condition = condition, TrueValue = a, FalseValue = b };

				if (condition.ReplaceNodes((n) => n == node.Left, CodeBuilder.CreateAssignment(a.CloneNode(), node.Left)) != 1)
					throw new InvalidOperationException();

				BindExpressionType(e, GetMostGenericType(node));
				node.ParentNode.Replace(node, e);
			}
		}

		private IType GetMostGenericType(BinaryExpression node)
		{
			return TypeSystemServices.GetMostGenericType(GetExpressionType(node.Left), GetExpressionType(node.Right));
		}

		protected InternalLocal DeclareTempLocal(IType localType)
		{
			return CodeBuilder.DeclareTempLocal(_currentMethod, localType);
		}

		private void BindLogicalOperatorCondition(BinaryExpression node)
		{
			node.Left = AssertBoolContext(node.Left);
			node.Right = AssertBoolContext(node.Right);
			BindExpressionType(node, GetMostGenericType(node));
		}

		Expression AssertBoolContext(Expression expression)
		{
			var type = GetExpressionType(expression);
			if (TypeSystemServices.IsNumberOrBool(type) || type.IsEnum)
				return expression;

			var op_Implicit = TypeSystemServices.FindImplicitConversionOperator(type, TypeSystemServices.BoolType);
			if (op_Implicit != null)
			{
				//return [| $op_Implicit($expression) |]
				return CodeBuilder.CreateMethodInvocation(op_Implicit, expression);
			}

			// nullable types can be used in bool context
			if (TypeSystemServices.IsNullable(type))
			{
				//return [| $(expression).HasValue |]
				return CodeBuilder.CreateMethodInvocation(expression, NameResolutionService.ResolveMethod(type, "get_HasValue"));
			}

			// string in a boolean context means string.IsNullOrEmpty (BOO-1035)
			if (TypeSystemServices.StringType == type)
			{
				//return [| not string.IsNullOrEmpty($expression) |]
				var notIsNullOrEmpty = new UnaryExpression(
					UnaryOperatorType.LogicalNot,
					CodeBuilder.CreateMethodInvocation(String_IsNullOrEmpty, expression));
				BindExpressionType(notIsNullOrEmpty, TypeSystemServices.BoolType);
				return notIsNullOrEmpty;
			}

			// reference types can be used in bool context
			if (!type.IsValueType)
				return expression;

			Error(CompilerErrorFactory.BoolExpressionRequired(expression, type));
			return expression;
		}

		IMethod String_IsNullOrEmpty
		{
			get
			{
				if (_String_IsNullOrEmpty != null)
					return _String_IsNullOrEmpty;
				return _String_IsNullOrEmpty = TypeSystemServices.Map(Methods.Of<string, bool>(string.IsNullOrEmpty));
			}
		}
	}
}
