using System;
using System.Collections;
using System.Text;

namespace Boo.Lang
{
	public delegate bool Predicate(object item);

	/// <summary>
	/// List.
	/// </summary>
	public class List : ICollection
	{
		ArrayList _list;
		
		public List()
		{
			_list = new ArrayList();
		}
		
		public List(int initialCapacity)
		{
			_list = new ArrayList(initialCapacity);
		}		                                     

		public List(params object[] items)
		{
			_list = new ArrayList(items);
		}
		
		public int Count
		{
			get
			{
				return _list.Count;
			}
		}
		
		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}
		
		public void CopyTo(Array target, int index)
		{
			_list.CopyTo(target, index);
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

		public object this[int index]
		{
			get
			{
				return _list[index];
			}
		}

		public List Add(object item)
		{
			_list.Add(item);
			return this;
		}

		public List AddUnique(object item)
		{
			if (!_list.Contains(item))
			{
				_list.Add(item);
			}
			return this;
		}

		public List Collect(Predicate condition)
		{
			if (null == condition)
			{
				throw new ArgumentNullException("condition");
			}

			List newList = new List();
			InnerCollect(newList, condition);			
			return newList;
		}		

		public List Collect(List target, Predicate condition)
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
		
		public Array ToArray(System.Type targetType)
		{
			return _list.ToArray(targetType);
		}
		
		public List Sort()
		{
			_list.Sort();
			return this;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();			
			for (int i=0; i<_list.Count; ++i)
			{
				if (i>0) { sb.Append(", "); }
				sb.Append(_list[i]);
			}
			return sb.ToString();
		}

		void InnerCollect(List target, Predicate condition)
		{
			foreach (object item in _list)
			{
				if (condition(item))
				{
					target.Add(item);
				}
			}
		}
	}
}
