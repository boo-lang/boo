using System;
using Boo.Ast;

namespace Boo.Ast.Impl
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Module"/> elements.
	/// </summary>
	/// <remarks>
	/// <b>ModuleCollection</b> provides an <see cref="System.Collections.ArrayList"/> 
	/// that is strongly typed for <see cref="Module"/> elements.
	/// </remarks> 
	[Serializable]
	public class ModuleCollectionImpl : NodeCollection
	{
		protected ModuleCollectionImpl()
		{
		}
		
		protected ModuleCollectionImpl(Node parent) : base(parent)
		{
		}
		
		public Module this[int index]
		{
			get
			{
				return (Module)InnerList[index];
			}
		}

		public void Add(Module item)
		{
			base.Add(item);			
		}
		
		public void Add(params Module[] items)
		{
			base.Add(items);			
		}
		
		public void Add(System.Collections.ICollection items)
		{
			foreach (Module item in items)
			{
				base.Add(item);
			}
		}
		
		public void Insert(int index, Module item)
		{
			base.Insert(index, item);
		}
		
		public void Replace(Module existing, Module newItem)
		{
			base.Replace(existing, newItem);
		}
		
		public new Module[] ToArray()
		{
			return (Module[])InnerList.ToArray(typeof(Module));
		}
	}
}
