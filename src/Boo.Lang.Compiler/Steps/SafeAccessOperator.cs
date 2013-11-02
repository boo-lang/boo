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

using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	public class CheckLiteralValues : AbstractFastVisitorCompilerStep
	{
		private bool _checked;

		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}

		override public void OnBlock(Block block)
		{
			var currentChecked = _checked;
			_checked = AstAnnotations.IsChecked(block, Parameters.Checked);

			Visit(block.Statements);

			_checked = currentChecked;
		}

		override public void OnArrayLiteralExpression(ArrayLiteralExpression node)
		{
			if (!_checked)
				return;

			base.OnArrayLiteralExpression(node);

			var expectedType = GetExpressionType(node).ElementType;
			if (!TypeSystemServices.IsPrimitiveNumber(expectedType))
				return;

			foreach (var item in node.Items)
			{
				var integerLiteral = item as IntegerLiteralExpression;
				if (integerLiteral != null)
				{
					AssertLiteralInRange(integerLiteral, expectedType);
					continue;
				}
			}
		}

		override public void OnBinaryExpression(BinaryExpression node)
		{
			if (!_checked)
				return;

			base.OnBinaryExpression(node);

			if (node.Operator != BinaryOperatorType.Assign
			    || node.Right.NodeType != NodeType.IntegerLiteralExpression)
				return;

			IType expectedType = GetExpressionType(node.Left);
			if (!TypeSystemServices.IsPrimitiveNumber(expectedType))
				return;

			AssertLiteralInRange((IntegerLiteralExpression) node.Right, expectedType);
		}

		override public void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!_checked)
				return;

			base.OnMethodInvocationExpression(node);

			if (0 == node.Arguments.Count)
				return;

			IMethod target = node.Target.Entity as IMethod;
			if (null == target)
				return;
			IParameter[] parameters = target.GetParameters();
			if (parameters.Length != node.Arguments.Count)
				return;

			for (int i = 0; i < parameters.Length; ++i)
			{
				if (node.Arguments[i].NodeType != NodeType.IntegerLiteralExpression)
					continue;
				if (!TypeSystemServices.IsPrimitiveNumber(parameters[i].Type))
					continue;

				AssertLiteralInRange((IntegerLiteralExpression) node.Arguments[i], parameters[i].Type);
			}
		}

		public override void OnExpressionStatement(ExpressionStatement node)
		{
			if (!_checked)
				return;

			base.OnExpressionStatement(node);

			IntegerLiteralExpression literal = node.Expression as IntegerLiteralExpression;
			if (null == literal)
				return;

			AssertLiteralInRange(literal, GetExpressionType(literal));
		}

		void AssertLiteralInRange(IntegerLiteralExpression literal, IType type)
		{
			bool ok = true;

			if (type == TypeSystemServices.ByteType)
				ok = (literal.Value >= byte.MinValue && literal.Value <= byte.MaxValue);
			else if (type == TypeSystemServices.SByteType)
				ok = (literal.Value >= sbyte.MinValue && literal.Value <= sbyte.MaxValue);
			else if (type == TypeSystemServices.ShortType)
				ok = (literal.Value >= short.MinValue && literal.Value <= short.MaxValue);
			else if (type == TypeSystemServices.UShortType)
				ok = (literal.Value >= ushort.MinValue && literal.Value <= ushort.MaxValue);
			else if (type == TypeSystemServices.IntType)
				ok = (literal.Value >= int.MinValue && literal.Value <= int.MaxValue);
			else if (type == TypeSystemServices.UIntType)
				ok = (literal.Value >= uint.MinValue && literal.Value <= uint.MaxValue);
			else if (type == TypeSystemServices.LongType)
				ok = (literal.Value >= long.MinValue && literal.Value <= long.MaxValue);

			if (!ok)
				Error(CompilerErrorFactory.ConstantCannotBeConverted(literal, type));
		}
	}
}

