#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
//
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;

namespace Boo.Lang.Ast
{
	/// <summary>
	/// Node collection base class.
	/// </summary>
	public class NodeCollection : ICollection
	{
		protected Node _parent;
		
		protected ArrayList _innerList = new ArrayList();

		protected NodeCollection(Node parent)
		{			
			_parent = parent;
		}

		protected NodeCollection()
		{
		}

		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}
		
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}
		
		public void CopyTo(Array array, int index)
		{
			_innerList.CopyTo(array, index);
		}
		
		public IEnumerator GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}
		
		public void Clear()
		{			
			_innerList.Clear();
		}
		
		public Node[] ToArray()
		{
			return (Node[])_innerList.ToArray(typeof(Node));
		}
		
		public Node GetNodeAt(int index)
		{
			return (Node)_innerList[index];
		}
		
		protected ArrayList InnerList
		{
			get
			{
				return _innerList;
			}
		}

		internal void InitializeParent(Node parent)
		{
			_parent = parent;
			foreach (Node node in InnerList)
			{
				node.InitializeParent(_parent);
			}
		}
		
		public void RemoveAt(int index)
		{
			//Node existing = (Node)InnerList[index];
			//existing.InitializeParent(null);
			InnerList.RemoveAt(index);
		}
		
		internal void ReplaceAt(int i, Node newItem)
		{
			//Node existing = (Node)InnerList[i];
			//existing.InitializeParent(null);
			_innerList[i] = newItem;
			Initialize(newItem);			
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
			for (int i=0; i<_innerList.Count; ++i)
			{
				if (_innerList[i] == existing)
				{
					ReplaceAt(i, newItem);
					return;
				}
			}			
			throw new ApplicationException(Boo.ResourceManager.Format("NodeNotInCollection", existing));
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

		public void Switch(IAstSwitcher switcher)
		{
			for (int i=0; i<InnerList.Count; ++i)
			{
				((Node)InnerList[i]).Switch(switcher);
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
