using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TypeReference"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>TypeReferenceCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="TypeReference"/> elements.
	/// </remarks> 
	[Serializable]
	public class TypeReferenceCollectionImpl : NodeCollection
	{
		protected TypeReferenceCollectionImpl()
		{
		}
		
		protected TypeReferenceCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public TypeReference this[int index]
		{
			get
			{
				return (TypeReference)InnerList[index];
			}
		}

		public void Add(TypeReference item)
		{
			base.Add(item);			
		}
		
		public void Add(params TypeReference[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (TypeReference item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, TypeReference item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(TypeReference existing, TypeReference newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public TypeReference[] ToArray()
		{
			return (TypeReference[])InnerList.ToArray(typeof(TypeReference));
		}
	}
}
