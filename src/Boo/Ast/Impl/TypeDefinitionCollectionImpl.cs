using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="TypeDefinition"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>TypeDefinitionCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="TypeDefinition"/> elements.
	/// </remarks> 
	[Serializable]
	public class TypeDefinitionCollectionImpl : NodeCollection
	{
		protected TypeDefinitionCollectionImpl()
		{
		}
		
		protected TypeDefinitionCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public TypeDefinition this[int index]
		{
			get
			{
				return (TypeDefinition)InnerList[index];
			}
		}

		public void Add(TypeDefinition item)
		{
			base.Add(item);			
		}
		
		public void Add(params TypeDefinition[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (TypeDefinition item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, TypeDefinition item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(TypeDefinition existing, TypeDefinition newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public TypeDefinition[] ToArray()
		{
			return (TypeDefinition[])InnerList.ToArray(typeof(TypeDefinition));
		}
	}
}
