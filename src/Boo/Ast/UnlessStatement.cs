using System;
using Boo.Ast.Impl;

namespace Boo.Ast
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
