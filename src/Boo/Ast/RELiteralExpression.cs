using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class RELiteralExpression : RELiteralExpressionImpl
	{		
		public RELiteralExpression()
		{
 		}
		
		public RELiteralExpression(string value) : base(value)
		{
		}
		
		public RELiteralExpression(antlr.Token token, string value) : base(token, value)
		{
		}
		
		internal RELiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal RELiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnRELiteralExpression(this);
		}
	}
}
