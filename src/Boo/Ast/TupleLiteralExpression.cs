using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class TupleLiteralExpression : TupleLiteralExpressionImpl
	{		
		public TupleLiteralExpression()
		{
 		}
		
		internal TupleLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal TupleLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTupleLiteralExpression(this);
		}
	}
}
