using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class SelfLiteralExpression : SelfLiteralExpressionImpl
	{		
		public SelfLiteralExpression()
		{
 		}
		
		internal SelfLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal SelfLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnSelfLiteralExpression(this);
		}
	}
}
