using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class RaiseStatement : RaiseStatementImpl
	{		
		public RaiseStatement()
		{
 		}
		
		public RaiseStatement(Expression exception) : base(exception)
		{
		}
		
		public RaiseStatement(antlr.Token token, Expression exception) : base(token, exception)
		{
		}
		
		internal RaiseStatement(antlr.Token token) : base(token)
		{
		}
		
		internal RaiseStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnRaiseStatement(this);
		}
	}
}
