using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="WhenClause"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>WhenClauseCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="WhenClause"/> elements.
	/// </remarks> 
	[Serializable]
	public class WhenClauseCollectionImpl : NodeCollection
	{
		protected WhenClauseCollectionImpl()
		{
		}
		
		protected WhenClauseCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public WhenClause this[int index]
		{
			get
			{
				return (WhenClause)InnerList[index];
			}
		}

		public void Add(WhenClause item)
		{
			base.Add(item);			
		}
		
		public void Add(params WhenClause[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (WhenClause item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, WhenClause item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(WhenClause existing, WhenClause newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new WhenClause[] ToArray()
		{
			return (WhenClause[])InnerList.ToArray(typeof(WhenClause));
		}
	}
}
