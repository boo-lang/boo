using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Attribute"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>AttributeCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Attribute"/> elements.
	/// </remarks> 
	[Serializable]
	public class AttributeCollectionImpl : NodeCollection
	{
		protected AttributeCollectionImpl()
		{
		}
		
		protected AttributeCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Attribute this[int index]
		{
			get
			{
				return (Attribute)InnerList[index];
			}
		}

		public void Add(Attribute item)
		{
			base.Add(item);			
		}
		
		public void Add(params Attribute[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Attribute item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Attribute item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Attribute existing, Attribute newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public Attribute[] ToArray()
		{
			return (Attribute[])InnerList.ToArray(typeof(Attribute));
		}
	}
}
