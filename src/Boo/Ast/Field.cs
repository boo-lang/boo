using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[Serializable]
	public class Field : FieldImpl
	{		
		public Field()
		{
 		}
		
		public Field(TypeReference type) : base(type)
		{
		}
		
		public Field(antlr.Token token, TypeReference type) : base(token, type)
		{
		}
		
		internal Field(antlr.Token token) : base(token)
		{
		}
		
		internal Field(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnField(this);
		}
	}
}
