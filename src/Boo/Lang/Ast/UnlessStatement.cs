using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[Serializable]
	public class UnlessStatement : UnlessStatementImpl
	{		
		public UnlessStatement()
		{
 		}
		
		public UnlessStatement(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnUnlessStatement(this);
		}
	}
}
