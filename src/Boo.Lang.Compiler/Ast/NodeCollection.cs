#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
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
		
		public bool Contains(Predicate condition)
		{
			return _list.Contains(condition);
		}
		
		public bool ContainsEntity(Boo.Lang.Compiler.TypeSystem.IEntity entity)
		{
			foreach (Node node in _list)
			{
				if (entity == node.Entity)
				{
					return true;
				}
			}
			return false;
		}
		
		public Node RemoveByEntity(Boo.Lang.Compiler.TypeSystem.IEntity entity)
		{
			if (null == entity)
			{
				throw new ArgumentNullException("entity");
			}
			for (int i=0; i<_list.Count; ++i)
			{
				Node node = (Node)_list[i];
				if (entity == node.Entity)
				{
					_list.RemoveAt(i);
					return node;
				}
			}
			return null;
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
		
		public void ClearTypeSystemBindings()
		{
			foreach (Node node in _list)
			{
				node.ClearTypeSystemBindings();
			}
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
		
		public void Reject(Predicate condition)
		{
			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}
			
			int index = 0;
			foreach (Node node in ToArray())
			{
				if (condition(node))
				{
					RemoveAt(index);
				}
				else
				{
					++index;
				}
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
			AssertNotNull("items", items);
			foreach (Node item in items)
			{
				AddNode(item);
			}
		}

		protected bool ReplaceNode(Node existing, Node newItem)
		{
			AssertNotNull("existing", existing);			
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
			AssertNotNull("item", item);
			if (null != _parent)
			{
				item.InitializeParent(_parent);
			}
		}
		
		private void AssertNotNull(string descrip, object o)
		{
			if (o == null)
			{
				throw new ArgumentException(
					String.Format("null reference for: {0}", descrip));
			}
		}
		
	}
}
