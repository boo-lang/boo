using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class WhileStatement : WhileStatementImpl
	{		
		public WhileStatement()
		{
			_statements = new StatementCollection(this);
 		}
		
		public WhileStatement(Expression condition) : base(condition)
		{
		}
		
		public WhileStatement(antlr.Token token, Expression condition) : base(token, condition)
		{
		}
		
		internal WhileStatement(antlr.Token token) : base(token)
		{
		}
		
		internal WhileStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnWhileStatement(this);
		}
	}
}
