using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class BreakStatement : BreakStatementImpl
	{		
		public BreakStatement()
		{
 		}
		
		internal BreakStatement(antlr.Token token) : base(token)
		{
		}
		
		internal BreakStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnBreakStatement(this);
		}
	}
}
