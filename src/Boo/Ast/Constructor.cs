using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Constructor : ConstructorImpl
	{		
		public Constructor()
		{
 		}
		
		internal Constructor(antlr.Token token) : base(token)
		{
		}
		
		internal Constructor(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnConstructor(this);
		}
	}
}
