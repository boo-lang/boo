using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ExpressionStatement : ExpressionStatementImpl
	{		
		public ExpressionStatement()
		{
 		}
		
		public ExpressionStatement(Expression expression) : base(expression)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnExpressionStatement(this);
		}
	}
}
