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
			return Join(", ");
		}
		
		public string Join(string separator)
		{
			StringBuilder sb = new StringBuilder();			
			for (int i=0; i<_list.Count; ++i)
			{
				if (i>0) { sb.Append(separator); }
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
