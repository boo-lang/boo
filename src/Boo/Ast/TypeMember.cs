using System;
using Boo.Ast.Impl;

namespace Boo.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(TypeDefinition))]
	[System.Xml.Serialization.XmlInclude(typeof(EnumMember))]
	[System.Xml.Serialization.XmlInclude(typeof(Field))]
	[System.Xml.Serialization.XmlInclude(typeof(Property))]
	[System.Xml.Serialization.XmlInclude(typeof(Method))]
	[Serializable]
	public abstract class TypeMember : TypeMemberImpl
	{		
		public TypeMember()
		{
			_attributes = new AttributeCollection(this);
 		}
		
		public TypeMember(TypeMemberModifiers modifiers, string name) : base(modifiers, name)
		{
		}
		
		public TypeMember(antlr.Token token, TypeMemberModifiers modifiers, string name) : base(token, modifiers, name)
		{
		}
		
		internal TypeMember(antlr.Token token) : base(token)
		{
		}
		
		internal TypeMember(Node lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
