using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class NullLiteralExpression : NullLiteralExpressionImpl
	{		
		public NullLiteralExpression()
		{
 		}
		
		internal NullLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal NullLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnNullLiteralExpression(this);
		}
	}
}
