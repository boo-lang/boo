using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ClassDefinition : ClassDefinitionImpl
	{		
		public ClassDefinition()
		{
 		}
		
		internal ClassDefinition(antlr.Token token) : base(token)
		{
		}
		
		internal ClassDefinition(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnClassDefinition(this);
		}
	}
}
