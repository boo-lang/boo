using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class SuperLiteralExpression : SuperLiteralExpressionImpl
	{		
		public SuperLiteralExpression()
		{
 		}
		
		internal SuperLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal SuperLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnSuperLiteralExpression(this);
		}
	}
}
