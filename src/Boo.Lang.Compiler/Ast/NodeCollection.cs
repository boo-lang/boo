#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.Collections;
using Boo.Lang;

namespace Boo.Lang.Compiler.Ast
{
	/// <summary>
	/// Node collection base class.
	/// </summary>
	public class NodeCollection : ICollection, ICloneable
	{
		protected Node _parent;
		
		protected List _list = new List();

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
				return _list.Count;
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
			_list.CopyTo(array, index);
		}
		
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		
		public void Clear()
		{			
			_list.Clear();
		}
		
		public Node[] ToArray()
		{
			return (Node[])_list.ToArray(typeof(Node));
		}
		
		public Node[] ToReverseArray()
		{
			Node[] array = ToArray();
			Array.Reverse(array);
			return array;
		}
		
		public Node[] Select(NodeType type)
		{
			List result = new List();
			foreach (Node node in _list)
			{
				if (node.NodeType == type)
				{
					result.Add(node);
				}
			}
			return (Node[])result.ToArray(typeof(Node));
		}
		
		public Node GetNodeAt(int index)
		{
			return (Node)_list[index];
		}
		
		public object Clone()
		{
			NodeCollection clone = (NodeCollection)Activator.CreateInstance(GetType());
			List cloneList = clone._list;
			foreach (Node node in _list)
			{
				cloneList.Add(node.Clone());
			}
			return clone;
		}
		
		protected List InnerList
		{
			get
			{
				return _list;
			}
		}

		internal void InitializeParent(Node parent)
		{
			_parent = parent;
			foreach (Node node in _list)
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
			_list[i] = newItem;
			Initialize(newItem);			
		}

		protected void AddNode(Node item)
		{
			Initialize(item);
			_list.Add(item);
		}

		protected void AddNodes(Node[] items)
		{
			Assert.AssertNotNull("items", items);
			foreach (Node item in items)
			{
				AddNode(item);
			}
		}

		protected bool ReplaceNode(Node existing, Node newItem)
		{
			Assert.AssertNotNull("existing", existing);			
			for (int i=0; i<_list.Count; ++i)
			{
				if (_list[i] == existing)
				{
					if (null == newItem)
					{
						RemoveAt(i);
					}
					else
					{
						ReplaceAt(i, newItem);
					}
					return true;
				}
			}			
			return false;
		}

		protected void InsertNode(int index, Node item)
		{			
			Initialize(item);
			InnerList.Insert(index, item);
		}

		public void Remove(Node item)
		{
			InnerList.Remove(item);
		}

		override public int GetHashCode()
		{
			return base.GetHashCode();
		}

		override public bool Equals(object rhs)
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
