using System;
using Boo.Lang.Ast.Impl;

namespace Boo.Lang.Ast
{
	[Serializable]
	public class TupleTypeReference : TupleTypeReferenceImpl
	{		
		public TupleTypeReference()
		{
 		}
		
		public TupleTypeReference(TypeReference elementType) : base(elementType)
		{
		}
		
		public TupleTypeReference(LexicalInfo lexicalInfo, TypeReference elementType) : base(lexicalInfo, elementType)
		{
		}
		
		public TupleTypeReference(LexicalInfo lexicalInfoProvider) : base(lexicalInfoProvider)
		{
		}
		
		public override void Switch(IAstSwitcher switcher)
		{
			switcher.OnTupleTypeReference(this);
		}
	}
}
