using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[Serializable]
	public class SimpleTypeReference : SimpleTypeReferenceImpl
	{		
		public SimpleTypeReference()
		{
 		}
		
		public SimpleTypeReference(string name) : base(name)
		{
		}
		
		public SimpleTypeReference(LexicalInfo lexicalInfo, string name) : base(lexicalInfo, name)
		{
		}
		
		public SimpleTypeReference(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnSimpleTypeReference(this);
		}
	}
}
