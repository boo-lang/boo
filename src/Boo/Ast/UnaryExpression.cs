using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class UnaryExpression : UnaryExpressionImpl
	{		
		public UnaryExpression()
		{
 		}
		
		public UnaryExpression(UnaryOperatorType operator_, Expression operand) : base(operator_, operand)
		{
		}
		
		public UnaryExpression(antlr.Token token, UnaryOperatorType operator_, Expression operand) : base(token, operator_, operand)
		{
		}
		
		internal UnaryExpression(antlr.Token token) : base(token)
		{
		}
		
		internal UnaryExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnUnaryExpression(this);
		}
	}
}
