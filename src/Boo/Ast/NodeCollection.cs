using System;
using System.Collections;

namespace Boo.Ast
{
	/// <summary>
	/// Classe base para colees de ns.
	/// </summary>
	public class NodeCollection : CollectionBase, Boo.Util.ISwitchable
	{
		protected Node _parent;

		protected NodeCollection(Node parent)
		{			
			_parent = parent;
		}

		protected NodeCollection()
		{
		}

		internal void InitializeParent(Node parent)
		{
			_parent = parent;
			foreach (Node node in InnerList)
			{
				node.InitializeParent(_parent);
			}
		}

		protected void Add(Node item)
		{
			Initialize(item);
			InnerList.Add(item);
		}

		protected void Add(Node[] items)
		{
			Assert.AssertNotNull("items", items);
			foreach (Node item in items)
			{
				Add(item);
			}
		}

		protected void Replace(Node existing, Node newItem)
		{
			Assert.AssertNotNull("existing", existing);
			Assert.AssertNotNull("newItem", newItem);
			for (int i=0; i<InnerList.Count; ++i)
			{
				if (InnerList[i] == existing)
				{
					InnerList[i] = newItem;
					Initialize(newItem);
					existing.InitializeParent(null);
					return;
				}
			}			
			throw new ApplicationException(string.Format("{0} no pertence a esta coleo.", existing));
		}

		protected void Insert(int index, Node item)
		{			
			Initialize(item);
			InnerList.Insert(index, item);
		}

		public void Remove(Node item)
		{
			InnerList.Remove(item);
		}

		public void Switch(Boo.Util.ISwitcher switcher)
		{
			Switch((IAstSwitcher)switcher);
		}

		public void Switch(IAstSwitcher switcher)
		{
			foreach (Node node in InnerList.ToArray())
			{
				node.Switch(switcher);
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object rhs)
		{
			NodeCollection other = rhs as NodeCollection;
			if (null == other)
			{
				return false;
			}
			if (InnerList.Count != other.Count)
			{
				return false;
			}
			for (int i=0; i<InnerList.Count; ++i)
			{
				if (!InnerList[i].Equals(other.InnerList[i]))
				{
					return false;
				}
			}
			return true;
		}

		void Initialize(Node item)
		{
			Assert.AssertNotNull("item", item);
			if (null != _parent)
			{
				item.InitializeParent(_parent);
			}
		}
	}
}
