using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Using"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>UsingCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Using"/> elements.
	/// </remarks> 
	[Serializable]
	public class UsingCollectionImpl : NodeCollection
	{
		protected UsingCollectionImpl()
		{
		}
		
		protected UsingCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Using this[int index]
		{
			get
			{
				return (Using)InnerList[index];
			}
		}

		public void Add(Using item)
		{
			base.Add(item);			
		}
		
		public void Add(params Using[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Using item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Using item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Using existing, Using newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new Using[] ToArray()
		{
			return (Using[])InnerList.ToArray(typeof(Using));
		}
	}
}
