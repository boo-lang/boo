using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class TypeReference : TypeReferenceImpl
	{		
		public TypeReference()
		{
 		}
		
		public TypeReference(string name) : base(name)
		{
		}
		
		public TypeReference(antlr.Token token, string name) : base(token, name)
		{
		}
		
		internal TypeReference(antlr.Token token) : base(token)
		{
		}
		
		internal TypeReference(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTypeReference(this);
		}
	}
}
