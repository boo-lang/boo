using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class StringFormattingExpression : StringFormattingExpressionImpl
	{		
		public StringFormattingExpression()
		{
			_arguments = new ExpressionCollection(this);
 		}
		
		public StringFormattingExpression(string template) : base(template)
		{
		}
		
		public StringFormattingExpression(antlr.Token token, string template) : base(token, template)
		{
		}
		
		internal StringFormattingExpression(antlr.Token token) : base(token)
		{
		}
		
		internal StringFormattingExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnStringFormattingExpression(this);
		}
	}
}
