using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ExpressionPair"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ExpressionPairCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="ExpressionPair"/> elements.
	/// </remarks> 
	[Serializable]
	public class ExpressionPairCollectionImpl : NodeCollection
	{
		protected ExpressionPairCollectionImpl()
		{
		}
		
		protected ExpressionPairCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public ExpressionPair this[int index]
		{
			get
			{
				return (ExpressionPair)InnerList[index];
			}
		}

		public void Add(ExpressionPair item)
		{
			base.Add(item);			
		}
		
		public void Add(params ExpressionPair[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (ExpressionPair item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, ExpressionPair item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(ExpressionPair existing, ExpressionPair newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new ExpressionPair[] ToArray()
		{
			return (ExpressionPair[])InnerList.ToArray(typeof(ExpressionPair));
		}
	}
}
