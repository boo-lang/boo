${header}
namespace Boo.Lang.Compiler.Ast.Impl
{
	using System;
	using Boo.Lang.Compiler.Ast;
<%

itemType = "Boo.Lang.Compiler.Ast." + model.GetCollectionItemType(node)

%>	
	[Serializable]
	[Boo.Lang.EnumeratorItemType(typeof(${itemType}))]
	public class ${node.Name}Impl : NodeCollection
	{
		protected ${node.Name}Impl()
		{
		}
		
		protected ${node.Name}Impl(Node parent) : base(parent)
		{
		}
		
		protected ${node.Name}Impl(Node parent, Boo.Lang.List list) : base(parent, list)
		{
		}
		
		public ${itemType} this[int index]
		{
			get
			{
				return (${itemType})InnerList[index];
			}
		}

		public void Add(${itemType} item)
		{
			base.AddNode(item);			
		}
		
		public void Extend(params ${itemType}[] items)
		{
			base.AddNodes(items);			
		}
		
		public void Extend(System.Collections.ICollection items)
		{
			foreach (${itemType} item in items)
			{
				base.AddNode(item);
			}
		}
		
		public void ExtendWithClones(System.Collections.ICollection items)
		{
			foreach (${itemType} item in items)
			{
				base.AddNode(item.CloneNode());
			}
		}
		
		public void Insert(int index, ${itemType} item)
		{
			base.InsertNode(index, item);
		}
		
		public bool Replace(${itemType} existing, ${itemType} newItem)
		{
			return base.ReplaceNode(existing, newItem);
		}
		
		public void ReplaceAt(int index, ${itemType} newItem)
		{
			base.ReplaceAt(index, newItem);
		}
		
		public ${itemType}Collection PopRange(int begin)
		{
			return new ${itemType}Collection(_parent, InnerList.PopRange(begin));
		}
		
		public new ${itemType}[] ToArray()
		{
			return (${itemType}[])InnerList.ToArray(typeof(${itemType}));
		}
	}
}

