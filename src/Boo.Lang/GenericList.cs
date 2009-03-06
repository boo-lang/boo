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
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	using System.Collections;
	using System.Collections.Generic;

	[Serializable]
	public class List<T> : IList<T>, IList, IEquatable<List<T>>
	{
		private static readonly T[] EmptyArray = new T[0];

		protected T[] _items;

		protected int _count;

		public List()
		{
			_items = EmptyArray;
		}

		public List(System.Collections.IEnumerable enumerable) : this()
		{
			Extend(enumerable);
		}

		public List(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity");
			}
			_items = new T[initialCapacity];
			_count = 0;
		}

		public List(T[] items, bool takeOwnership)
		{
			if (null == items)
			{
				throw new ArgumentNullException("items");
			}

			if (takeOwnership)
			{
				_items = items;
			}
			else
			{
				_items = (T[])items.Clone();
			}
			_count = items.Length;
		}

		public static List<T> operator*(List<T> lhs, int count)
		{
			return lhs.Multiply(count);
		}

		public static List<T> operator*(int count, List<T> rhs)
		{
			return rhs.Multiply(count);
		}

		public static List<T> operator+(List<T> lhs, System.Collections.IEnumerable rhs)
		{
			List<T> result = lhs.NewConcreteList(lhs.ToArray(), true);
			result.Extend(rhs);
			return result;
		}

		public List<T> Multiply(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}

			T[] items = new T[_count*count];
			for (int i=0; i<count; ++i)
			{
				Array.Copy(_items, 0, items, i*_count, _count);
			}
			return NewConcreteList(items, true);
		}

		protected virtual List<T> NewConcreteList(T[] items, bool takeOwnership)
		{
			return new List<T>(items, takeOwnership);
		}

		public IEnumerable<T> Reversed
		{
			get
			{
				for (int i=_count-1; i>=0; --i)
				{
					yield return _items[i];
				}
			}
		}

		public int Count
		{
			get { return _count; }
		}

		void ICollection<T>.Add(T item)
		{
			Push(item);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>) this).GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			int originalCount = _count;
			T[] originalItems = _items;
			for (int i = 0; i < _count; ++i)
			{
				if (originalCount != _count || originalItems != _items)
				{
					throw new InvalidOperationException(ResourceManager.GetString("ListWasModified"));
				}
				yield return _items[i];
			}
		}

		public void CopyTo(T[] target, int index)
		{
			Array.Copy(_items, 0, target, index, _count);
		}

		public bool IsSynchronized
		{
			get { return false;  }
		}

		public object SyncRoot
		{
			get { return _items; }
		}

		public bool IsReadOnly
		{
			get { return false;  }
		}

		public T this[int index]
		{
			get
			{
				return _items[CheckIndex(NormalizeIndex(index))];
			}

			set
			{
				_items[CheckIndex(NormalizeIndex(index))] = value;
			}
		}

		public List<T> Push(T item)
		{
			return Add(item);
		}

		public virtual List<T> Add(T item)
		{
			EnsureCapacity(_count+1);
			_items[_count] = item;
			++_count;
			return this;
		}

		public List<T> AddUnique(T item)
		{
			if (!Contains(item))
			{
				Add(item);
			}
			return this;
		}

		public List<T> Extend(System.Collections.IEnumerable enumerable)
		{
			foreach (T item in enumerable)
			{
				Add(item);
			}
			return this;
		}

		public List<T> ExtendUnique(System.Collections.IEnumerable enumerable)
		{
			foreach (T item in enumerable)
			{
				AddUnique(item);
			}
			return this;
		}

		public List<T> Collect(System.Predicate<T> condition)
		{
			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}

			List<T> newList = NewConcreteList(new T[0], true);
			InnerCollect(newList, condition);
			return newList;
		}

		public List<T> Collect(List<T> target, System.Predicate<T> condition)
		{
			if (null == target)
			{
				throw new ArgumentNullException("target");
			}

			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}

			InnerCollect(target, condition);
			return target;
		}

		public T[] ToArray()
		{
			T[] target = new T[_count];
			CopyTo(target, 0);
			return target;
		}

		public T[] ToArray(T[] array)
		{
			CopyTo(array, 0);
			return array;
		}

		public List<T> Sort()
		{
			Array.Sort(_items, 0, _count, BooComparer.Default);
			return this;
		}

		public List<T> Sort(System.Collections.IComparer comparer)
		{
			Array.Sort(_items, 0, _count, comparer);
			return this;
		}

		private sealed class ComparisonComparer : IComparer<T>
		{
			private readonly Comparison<T> _comparison;

			public ComparisonComparer(Comparison<T> comparison)
			{
				_comparison = comparison;
			}

			#region IComparer<T> Members

			public int Compare(T x, T y)
			{
				return _comparison(x, y);
			}

			#endregion
		}

		public List<T> Sort(System.Comparison<T> comparison)
		{
			return Sort(new ComparisonComparer(comparison));
		}

		public List<T> Sort(System.Collections.Generic.IComparer<T> comparer)
		{
			Array.Sort(_items, 0, _count, comparer);
			return this;
		}

		private sealed class ComparerImpl : System.Collections.IComparer
		{
			Comparer _comparer;

			public ComparerImpl(Comparer comparer)
			{
				_comparer = comparer;
			}

			public int Compare(object lhs, object rhs)
			{
				return _comparer(lhs, rhs);
			}
		}

		public List<T> Sort(Comparer comparer)
		{
			if (null == comparer)
			{
				throw new ArgumentNullException("comparer");
			}
			return Sort(new ComparerImpl(comparer));
		}

		override public string ToString()
		{
			return "[" + Join(", ") + "]";
		}

		public string Join(string separator)
		{
			return Builtins.join(this, separator);
		}

		override public int GetHashCode()
		{
			int hash = _count;

			for (int i=0; i<_count; ++i)
			{
				T item = _items[i];
				if (null != item)
				{
					hash ^= item.GetHashCode();
				}
			}
			return hash;
		}

		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			List<T> list = other as List<T>;
			return Equals(list);
		}

		public bool Equals(List<T> other)
		{
			if (null == other) return false;
			if (this == other) return true;
			if (_count != other.Count) return false;

			for (int i=0; i < _count; ++i)
			{
				if (!RuntimeServices.EqualityOperator(_items[i], other[i]))
					return false;
			}
			return true;
		}

		public void Clear()
		{
			for (int i=0; i<_count; ++i)
			{
				_items[i] = default(T);
			}
			_count = 0;
		}

		public List<T> GetRange(int begin)
		{
			return InnerGetRange(AdjustIndex(NormalizeIndex(begin)), _count);
		}

		public List<T> GetRange(int begin, int end)
		{
			return InnerGetRange(
					AdjustIndex(NormalizeIndex(begin)),
					AdjustIndex(NormalizeIndex(end)));
		}

		public bool Contains(T item)
		{
			return -1 != IndexOf(item);
		}

		public bool Contains(System.Predicate<T> condition)
		{
			return -1 != IndexOf(condition);
		}

		public bool Find(System.Predicate<T> condition, out T found)
		{
			int index = IndexOf(condition);
			if (-1 != index)
			{
				found = _items[index];
				return true;
			}
			found = default(T);
			return false;
		}

		public List<T> FindAll(System.Predicate<T> condition)
		{
			List<T> result = NewConcreteList(new T[0], true);
			foreach (T item in this)
			{
				if (condition(item)) result.Add(item);
			}
			return result;
		}

		public int IndexOf(System.Predicate<T> condition)
		{
			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}

			for (int i=0; i<_count; ++i)
			{
				if (condition(_items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(T item)
		{
			for (int i=0; i<_count; ++i)
			{
				if (RuntimeServices.EqualityOperator(_items[i], item))
				{
					return i;
				}
			}
			return -1;
		}

		public List<T> Insert(int index, T item)
		{
			int actual = NormalizeIndex(index);
			EnsureCapacity(Math.Max(_count, actual) + 1);

			if (actual < _count)
			{
				Array.Copy(_items, actual, _items, actual+1, _count-actual);
			}

			_items[actual] = item;
			++_count;
			return this;
		}

		public T Pop()
		{
			return Pop(-1);
		}

		public T Pop(int index)
		{
			int actualIndex = CheckIndex(NormalizeIndex(index));
			T item = _items[actualIndex];
			InnerRemoveAt(actualIndex);
			return item;
		}

		public List<T> PopRange(int begin)
		{
			int actualIndex = AdjustIndex(NormalizeIndex(begin));
			List<T> range = InnerGetRange(actualIndex, AdjustIndex(NormalizeIndex(_count)));
			for (int i=actualIndex; i<_count; ++i)
			{
				_items[i] = default(T);
			}
			_count = actualIndex;
			return range;
		}

		public List<T> RemoveAll(System.Predicate<T> match)
		{
			if (null == match) throw new ArgumentNullException("match");
			for (int i=0; i<_count; ++i)
			{
				if (match(_items[i])) InnerRemoveAt(i--);
			}
			return this;
		}

		public List<T> Remove(T item)
		{
			InnerRemove(item);
			return this;
		}

		public List<T> RemoveAt(int index)
		{
			InnerRemoveAt(CheckIndex(NormalizeIndex(index)));
			return this;
		}

		void IList<T>.Insert(int index, T item)
		{
			Insert(index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			InnerRemoveAt(CheckIndex(NormalizeIndex(index)));
		}

		bool ICollection<T>.Remove(T item)
		{
			return InnerRemove(item);
		}

		void EnsureCapacity(int minCapacity)
		{
			if (minCapacity > _items.Length)
			{
				T[] items = NewArray(minCapacity);
				Array.Copy(_items, 0, items, 0, _count);
				_items = items;
			}
		}

		T[] NewArray(int minCapacity)
		{
			int newLen = Math.Max(1, _items.Length)*2;
			return new T[Math.Max(newLen, minCapacity)];
		}

		void InnerRemoveAt(int index)
		{
			--_count;
			_items[index] = default(T);
			if (index != _count)
			{
				Array.Copy(_items, index+1, _items, index, _count-index);
			}
		}

		bool InnerRemove(T item)
		{
			int index = IndexOf(item);
			if (index != -1)
			{
				InnerRemoveAt(index);
				return true;
			}
			return false;
		}

		void InnerCollect(List<T> target, System.Predicate<T> condition)
		{
			for (int i=0; i<_count; ++i)
			{
				T item = _items[i];
				if (condition(item))
				{
					target.Add(item);
				}
			}
		}

		List<T> InnerGetRange(int begin, int end)
		{
			int targetLen = end-begin;
			if (targetLen > 0)
			{
				T[] target = new T[targetLen];
				Array.Copy(_items, begin, target, 0, targetLen);
				return NewConcreteList(target, true);
			}
			return NewConcreteList(new T[0], true);
		}

		int AdjustIndex(int index)
		{
			if (index > _count)
			{
				return _count;
			}
			if (index < 0)
			{
				return 0;
			}
			return index;
		}

		int CheckIndex(int index)
		{
			if (index >= _count)
			{
				throw new IndexOutOfRangeException();
			}
			return index;
		}

		int NormalizeIndex(int index)
		{
			if (index < 0)
			{
				index += _count;
			}
			return index;
		}



		#region IList Members

		int IList.Add(object value)
		{
			Add((T)value);
			return Count - 1;
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, Coerce(value));
		}

		private static T Coerce(object value)
		{
			if (value is T) return (T) value;
			return (T)RuntimeServices.Coerce(value, typeof(T));
		}

		void IList.Remove(object value)
		{
			Remove(Coerce(value));
		}

		int IList.IndexOf(object value)
		{
			return IndexOf(Coerce(value));
		}

		bool IList.Contains(object value)
		{
			return Contains(Coerce(value));
		}

		object IList.this[int index]
		{
			get { return this[index];  }
			set { this[index] = Coerce(value);  }
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			Array.Copy(_items, 0, array, index, _count);
		}

		#endregion
	}
}
