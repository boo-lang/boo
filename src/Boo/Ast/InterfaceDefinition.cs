using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class InterfaceDefinition : InterfaceDefinitionImpl
	{		
		public InterfaceDefinition()
		{
 		}
		
		internal InterfaceDefinition(antlr.Token token) : base(token)
		{
		}
		
		internal InterfaceDefinition(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnInterfaceDefinition(this);
		}
	}
}
