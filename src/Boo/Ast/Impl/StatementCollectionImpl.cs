using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Statement"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>StatementCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Statement"/> elements.
	/// </remarks> 
	[Serializable]
	public class StatementCollectionImpl : NodeCollection
	{
		protected StatementCollectionImpl()
		{
		}
		
		protected StatementCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Statement this[int index]
		{
			get
			{
				return (Statement)InnerList[index];
			}
		}

		public void Add(Statement item)
		{
			base.Add(item);			
		}
		
		public void Add(params Statement[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Statement item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Statement item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Statement existing, Statement newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new Statement[] ToArray()
		{
			return (Statement[])InnerList.ToArray(typeof(Statement));
		}
	}
}
