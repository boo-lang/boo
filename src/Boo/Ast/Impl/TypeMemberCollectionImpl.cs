using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TypeMember"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>TypeMemberCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="TypeMember"/> elements.
	/// </remarks> 
	[Serializable]
	public class TypeMemberCollectionImpl : NodeCollection
	{
		protected TypeMemberCollectionImpl()
		{
		}
		
		protected TypeMemberCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public TypeMember this[int index]
		{
			get
			{
				return (TypeMember)InnerList[index];
			}
		}

		public void Add(TypeMember item)
		{
			base.Add(item);			
		}
		
		public void Add(params TypeMember[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (TypeMember item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, TypeMember item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(TypeMember existing, TypeMember newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new TypeMember[] ToArray()
		{
			return (TypeMember[])InnerList.ToArray(typeof(TypeMember));
		}
	}
}
