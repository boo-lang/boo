using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Declaration : DeclarationImpl
	{		
		public Declaration()
		{
 		}
		
		public Declaration(string name, TypeReference type) : base(name, type)
		{
		}
		
		public Declaration(antlr.Token token, string name, TypeReference type) : base(token, name, type)
		{
		}
		
		internal Declaration(antlr.Token token) : base(token)
		{
		}
		
		internal Declaration(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnDeclaration(this);
		}
	}
}
