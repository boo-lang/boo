using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class IntegerLiteralExpression : IntegerLiteralExpressionImpl
	{		
		public IntegerLiteralExpression()
		{
 		}
		
		public IntegerLiteralExpression(string value) : base(value)
		{
		}
		
		public IntegerLiteralExpression(antlr.Token token, string value) : base(token, value)
		{
		}
		
		internal IntegerLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal IntegerLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnIntegerLiteralExpression(this);
		}
	}
}
