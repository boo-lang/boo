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
		
		public ExpressionStatement(antlr.Token token, Expression expression) : base(token, expression)
		{
		}
		
		internal ExpressionStatement(antlr.Token token) : base(token)
		{
		}
		
		internal ExpressionStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnExpressionStatement(this);
		}
	}
}
