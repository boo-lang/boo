using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ContinueStatement : ContinueStatementImpl
	{		
		public ContinueStatement()
		{
 		}
		
		internal ContinueStatement(antlr.Token token) : base(token)
		{
		}
		
		internal ContinueStatement(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnContinueStatement(this);
		}
	}
}
