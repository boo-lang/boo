using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class TernaryExpression : TernaryExpressionImpl
	{		
		public TernaryExpression()
		{
 		}
		
		public TernaryExpression(Expression condition, Expression trueExpression, Expression falseExpression) : base(condition, trueExpression, falseExpression)
		{
		}
		
		public TernaryExpression(antlr.Token token, Expression condition, Expression trueExpression, Expression falseExpression) : base(token, condition, trueExpression, falseExpression)
		{
		}
		
		internal TernaryExpression(antlr.Token token) : base(token)
		{
		}
		
		internal TernaryExpression(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTernaryExpression(this);
		}
	}
}
