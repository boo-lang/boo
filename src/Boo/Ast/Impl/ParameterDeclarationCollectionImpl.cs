using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="ParameterDeclaration"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ParameterDeclarationCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="ParameterDeclaration"/> elements.
	/// </remarks> 
	[Serializable]
	public class ParameterDeclarationCollectionImpl : NodeCollection
	{
		protected ParameterDeclarationCollectionImpl()
		{
		}
		
		protected ParameterDeclarationCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public ParameterDeclaration this[int index]
		{
			get
			{
				return (ParameterDeclaration)InnerList[index];
			}
		}

		public void Add(ParameterDeclaration item)
		{
			base.Add(item);			
		}
		
		public void Add(params ParameterDeclaration[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (ParameterDeclaration item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, ParameterDeclaration item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(ParameterDeclaration existing, ParameterDeclaration newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new ParameterDeclaration[] ToArray()
		{
			return (ParameterDeclaration[])InnerList.ToArray(typeof(ParameterDeclaration));
		}
	}
}
