using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class WhenClause : WhenClauseImpl
	{		
		public WhenClause()
		{
 		}
		
		public WhenClause(Expression condition) : base(condition)
		{
		}
		
		public WhenClause(antlr.Token token, Expression condition) : base(token, condition)
		{
		}
		
		internal WhenClause(antlr.Token token) : base(token)
		{
		}
		
		internal WhenClause(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnWhenClause(this);
		}
	}
}
