using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ExpressionPair : ExpressionPairImpl
	{		
		public ExpressionPair()
		{
 		}
		
		public ExpressionPair(Expression first, Expression second) : base(first, second)
		{
		}
		
		public ExpressionPair(antlr.Token token, Expression first, Expression second) : base(token, first, second)
		{
		}
		
		internal ExpressionPair(antlr.Token token) : base(token)
		{
		}
		
		internal ExpressionPair(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnExpressionPair(this);
		}
	}
}
