using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Local"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>LocalCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Local"/> elements.
	/// </remarks> 
	[Serializable]
	public class LocalCollectionImpl : NodeCollection
	{
		protected LocalCollectionImpl()
		{
		}
		
		protected LocalCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Local this[int index]
		{
			get
			{
				return (Local)InnerList[index];
			}
		}

		public void Add(Local item)
		{
			base.Add(item);			
		}
		
		public void Add(params Local[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Local item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Local item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Local existing, Local newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public Local[] ToArray()
		{
			return (Local[])InnerList.ToArray(typeof(Local));
		}
	}
}
