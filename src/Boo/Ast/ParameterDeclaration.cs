using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class ParameterDeclaration : ParameterDeclarationImpl
	{		
		public ParameterDeclaration()
		{
			_attributes = new AttributeCollection(this);
 		}
		
		public ParameterDeclaration(string name, TypeReference type) : base(name, type)
		{
		}
		
		public ParameterDeclaration(antlr.Token token, string name, TypeReference type) : base(token, name, type)
		{
		}
		
		internal ParameterDeclaration(antlr.Token token) : base(token)
		{
		}
		
		internal ParameterDeclaration(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnParameterDeclaration(this);
		}
	}
}
