using System;

namespace Boo.Ast
{
	public class TypeMemberCollection : Boo.Ast.Impl.TypeMemberCollectionImpl
	{
		public TypeMemberCollection()
		{
		}
		
		public TypeMemberCollection(Boo.Ast.Node parent) : base(parent)
		{
		}
		
		public TypeMember this[string name]
		{
			get
			{
				foreach (TypeMember member in InnerList)
				{
					if (member.Name == name)
					{
						return member;
					}
				}
				return null;
			}
		}
	}
}
