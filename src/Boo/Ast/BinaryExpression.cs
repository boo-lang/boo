using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class BinaryExpression : BinaryExpressionImpl
	{		
		public BinaryExpression()
		{
 		}
		
		public BinaryExpression(BinaryOperatorType operator_, Expression left, Expression right) : base(operator_, left, right)
		{
		}
		
		public BinaryExpression(antlr.Token token, BinaryOperatorType operator_, Expression left, Expression right) : base(token, operator_, left, right)
		{
		}
		
		internal BinaryExpression(antlr.Token token) : base(token)
		{
		}
		
		internal BinaryExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnBinaryExpression(this);
		}
	}
}
