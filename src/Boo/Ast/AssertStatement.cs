using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class AssertStatement : AssertStatementImpl
	{		
		public AssertStatement()
		{
 		}
		
		public AssertStatement(Expression condition, Expression message) : base(condition, message)
		{
		}
		
		public AssertStatement(antlr.Token token, Expression condition, Expression message) : base(token, condition, message)
		{
		}
		
		internal AssertStatement(antlr.Token token) : base(token)
		{
		}
		
		internal AssertStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnAssertStatement(this);
		}
	}
}
