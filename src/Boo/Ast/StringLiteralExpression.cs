using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class StringLiteralExpression : StringLiteralExpressionImpl
	{		
		public StringLiteralExpression()
		{
 		}
		
		public StringLiteralExpression(string value) : base(value)
		{
		}
		
		public StringLiteralExpression(antlr.Token token, string value) : base(token, value)
		{
		}
		
		internal StringLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal StringLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnStringLiteralExpression(this);
		}
	}
}
