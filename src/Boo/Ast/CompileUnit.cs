using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class CompileUnit : CompileUnitImpl
	{		
		public CompileUnit()
		{
			_modules = new ModuleCollection(this);
 		}
		
		internal CompileUnit(antlr.Token token) : base(token)
		{
		}
		
		internal CompileUnit(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnCompileUnit(this);
		}
	}
}
