using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Expression"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ExpressionCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Expression"/> elements.
	/// </remarks> 
	[Serializable]
	public class ExpressionCollectionImpl : NodeCollection
	{
		protected ExpressionCollectionImpl()
		{
		}
		
		protected ExpressionCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Expression this[int index]
		{
			get
			{
				return (Expression)InnerList[index];
			}
		}

		public void Add(Expression item)
		{
			base.Add(item);			
		}
		
		public void Add(params Expression[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Expression item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Expression item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Expression existing, Expression newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public Expression[] ToArray()
		{
			return (Expression[])InnerList.ToArray(typeof(Expression));
		}
	}
}
