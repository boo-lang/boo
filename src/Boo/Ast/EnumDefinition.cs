using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class EnumDefinition : EnumDefinitionImpl
	{		
		public EnumDefinition()
		{
 		}
		
		internal EnumDefinition(antlr.Token token) : base(token)
		{
		}
		
		internal EnumDefinition(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnEnumDefinition(this);
		}
	}
}
