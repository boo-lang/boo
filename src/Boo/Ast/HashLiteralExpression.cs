using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class HashLiteralExpression : HashLiteralExpressionImpl
	{		
		public HashLiteralExpression()
		{
			_items = new ExpressionPairCollection(this);
 		}
		
		internal HashLiteralExpression(antlr.Token token) : base(token)
		{
		}
		
		internal HashLiteralExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnHashLiteralExpression(this);
		}
	}
}
