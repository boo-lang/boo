using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[System.Xml.Serialization.XmlInclude(typeof(SimpleTypeReference))]
	[System.Xml.Serialization.XmlInclude(typeof(TupleTypeReference))]
	[Serializable]
	public abstract class TypeReference : TypeReferenceImpl
	{		
		public TypeReference()
		{
 		}
		
		public TypeReference(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
	}
}
