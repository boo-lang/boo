using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Declaration"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>DeclarationCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Declaration"/> elements.
	/// </remarks> 
	[Serializable]
	public class DeclarationCollectionImpl : NodeCollection
	{
		protected DeclarationCollectionImpl()
		{
		}
		
		protected DeclarationCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Declaration this[int index]
		{
			get
			{
				return (Declaration)InnerList[index];
			}
		}

		public void Add(Declaration item)
		{
			base.Add(item);			
		}
		
		public void Add(params Declaration[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Declaration item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Declaration item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Declaration existing, Declaration newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new Declaration[] ToArray()
		{
			return (Declaration[])InnerList.ToArray(typeof(Declaration));
		}
	}
}
