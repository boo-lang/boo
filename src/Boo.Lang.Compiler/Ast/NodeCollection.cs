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
using System.Collections.Generic;
using Boo.Lang;

namespace Boo.Lang.Compiler.Ast
{
	
	/// <summary>
	/// Node collection base class.
	/// </summary>
	public class NodeCollection<T> : ICollection<T>, ICollection, ICloneable, IEquatable<NodeCollection<T>>
		where T : Node
	{
		protected Node _parent;

		protected List<T> _list;

		protected NodeCollection()
		{
			_list = new List<T>();
		}

		protected NodeCollection(Node parent)
		{
			_parent = parent;
			_list = new List<T>();
		}

		protected NodeCollection(Node parent, IEnumerable<T> list)
		{
			if (null == list) throw new ArgumentNullException("list");

			_parent = parent;
			_list = new List<T>(list);
		}

		public T this[int index]
		{
			get { return _list[index]; }
			set { _list[index] = value; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public Node ParentNode
		{
			get { return _parent; }
		}

		public bool IsEmpty
		{
			get { return (null == _list) || (0 == _list.Count); }
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_list).CopyTo(array, index);
		}

		public void CopyTo(T[] array, int index)
		{
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public void Clear()
		{
			_list.Clear();
		}

		public T[] ToArray()
		{
			return _list.ToArray();
		}

		public T[] ToReverseArray()
		{
			T[] array = ToArray();
			Array.Reverse(array);
			return array;
		}

		public IEnumerable<TNode> OfType<TNode>() where TNode : Node
		{
			foreach (Node node in _list)
			{
				TNode match = node as TNode;
				if (null != match)
					yield return match;
			}
		}

		public IEnumerable<T> Except<UnwantedNodeType>() where UnwantedNodeType : T
		{
			foreach (T node in _list)
				if (!(node is UnwantedNodeType))
					yield return node;
		}

		public IEnumerable<T> Except<UnwantedNodeType,UnwantedNodeType2>()
			where UnwantedNodeType : T
			where UnwantedNodeType2 : T
		{
			foreach (T node in _list)
				if (!(node is UnwantedNodeType) && !(node is UnwantedNodeType2))
					yield return node;
		}

		public T[] Select(NodeType type)
		{
			List<T> result = new List<T>();
			foreach (T node in _list)
				if (node.NodeType == type)
					result.Add(node);
			return result.ToArray();
		}

		protected IEnumerable InternalPopRange(int begin)
		{
			return _list.PopRange(begin);
		}

		public bool Contains(T node)
		{
			foreach (T n in _list)
			{
				if (n == node) return true;
			}
			return false;
		}

		[Obsolete("Use Contains(T node) instead.")]
		public bool ContainsNode(T node)
		{
			return Contains(node);
		}

		public bool Contains(System.Predicate<T> condition)
		{
			return _list.Contains(condition);
		}

		public bool ContainsEntity(Boo.Lang.Compiler.TypeSystem.IEntity entity)
		{
			foreach (T node in _list)
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
			for (int i = 0; i < _list.Count; ++i)
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

		public object Clone()
		{
			NodeCollection<T> clone = (NodeCollection<T>)Activator.CreateInstance(GetType());
			List<T> cloneList = clone._list;
			foreach (T node in _list)
			{
				cloneList.Add((T)node.CloneNode());
			}
			return clone;
		}

		public void ClearTypeSystemBindings()
		{
			foreach (T node in _list)
			{
				node.ClearTypeSystemBindings();
			}
		}

		protected List<T> InnerList
		{
			get
			{
				return _list;
			}
		}

		internal void InitializeParent(Node parent)
		{
			_parent = parent;
			foreach (T node in _list)
			{
				node.InitializeParent(_parent);
			}
		}

		public void Reject(System.Predicate<T> condition)
		{
			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}

			int index = 0;
			foreach (T node in ToArray())
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

		public void ExtendWithClones(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				Add((T)item.CloneNode());
			}
		}

		public void ReplaceAt(int i, T newItem)
		{
			//Node existing = (Node)InnerList[i];
			//existing.InitializeParent(null);
			_list[i] = newItem;
			Initialize(newItem);
		}

		public void Add(T item)
		{
			Initialize(item);
			_list.Add(item);
		}

		public void Extend(IEnumerable<T> items)
		{
			AssertNotNull("items", items);
			foreach (T item in items)
			{
				Add(item);
			}
		}

		public bool Replace(T existing, T newItem)
		{
			AssertNotNull("existing", existing);
			for (int i = 0; i < _list.Count; ++i)
			{
				if (this[i] == existing)
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

		public void Insert(int index, T item)
		{
			Initialize(item);
			InnerList.Insert(index, item);
		}

		public bool Remove(T item)
		{
			return ((ICollection<T>)_list).Remove(item);
		}

		override public int GetHashCode()
		{
			return _list.GetHashCode();
		}

		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			NodeCollection<T> collection = other as NodeCollection<T>;
			return Equals(collection);
		}

		public bool Equals(NodeCollection<T> other)
		{
			if (null == other) return false;
			if (this == other) return true;
			if (InnerList.Count != other.Count) return false;

			IEnumerator<T> enumerator = other.GetEnumerator();
			foreach (T mine in this)
			{
				if (!enumerator.MoveNext()) return false;
				if (!mine.Equals(enumerator.Current)) return false;
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

		public NodeCollection<TNew> Cast<TNew>() where TNew : Node
		{
			return Cast<TNew>(0);
		}

		public NodeCollection<TNew> Cast<TNew>(int index) where TNew : Node
		{
			if (index < 0 || index > Count)
				throw new ArgumentOutOfRangeException("index");

			if (0 == (Count - index)) //empty or no item to cast
				return Empty<TNew>(ParentNode);

			NodeCollection<TNew> nodes = new NodeCollection<TNew>(ParentNode);
			int i = -1;

			foreach (T node in _list)
			{
				++i;
				if (i < index)
					continue;

				TNew cnode = node as TNew;
				if (null == cnode)
				{
					ExpressionStatement es = node as ExpressionStatement;
					if (null != es)
						cnode = es.Expression as TNew;
				}
				if (null == cnode)
					throw new InvalidCastException(
						string.Format("Cannot cast item #{0} from `{1}` to `{2}`",
						              i+1, node.GetType(), typeof(TNew)));

				nodes.Add(cnode);
			}
			return nodes;
		}

		public static NodeCollection<TNode> Empty<TNode>(Node parent) where TNode : Node
		{
			//TODO: cache?
			return new NodeCollection<TNode>(parent);
		}

		public bool StartsWith<TNode>() where TNode : T
		{
			return !IsEmpty && (First is TNode);
		}

		public bool EndsWith<TNode>() where TNode : T
		{
			return !IsEmpty && (Last is TNode);
		}

		public T First
		{
			get { return (IsEmpty ? null : _list[0]); }
		}

		public T Last
		{
			get { return (IsEmpty ? null : _list[Count - 1]); }
		}
	}
}
