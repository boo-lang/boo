using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[Serializable]
	public class MacroStatement : MacroStatementImpl
	{		
		public MacroStatement()
		{
 		}
		
		public MacroStatement(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override string ToString()
		{
			return _name;
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnMacroStatement(this);
		}
	}
}
