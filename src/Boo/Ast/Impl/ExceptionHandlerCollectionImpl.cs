using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ExceptionHandler"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ExceptionHandlerCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="ExceptionHandler"/> elements.
	/// </remarks> 
	[Serializable]
	public class ExceptionHandlerCollectionImpl : NodeCollection
	{
		protected ExceptionHandlerCollectionImpl()
		{
		}
		
		protected ExceptionHandlerCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public ExceptionHandler this[int index]
		{
			get
			{
				return (ExceptionHandler)InnerList[index];
			}
		}

		public void Add(ExceptionHandler item)
		{
			base.Add(item);			
		}
		
		public void Add(params ExceptionHandler[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (ExceptionHandler item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, ExceptionHandler item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(ExceptionHandler existing, ExceptionHandler newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public ExceptionHandler[] ToArray()
		{
			return (ExceptionHandler[])InnerList.ToArray(typeof(ExceptionHandler));
		}
	}
}
