using System;
using System.Collections;
using System.Text;

namespace Boo.Lang
{
	public delegate bool Predicate(object item);

	/// <summary>
	/// List.
	/// </summary>
	public class List : CollectionBase
	{
		public List()
		{
		}

		public List(params object[] items)
		{
			InnerList.AddRange(items);
		}

		public object this[int index]
		{
			get
			{
				return InnerList[index];
			}
		}

		public List Add(object item)
		{
			InnerList.Add(item);
			return this;
		}

		public List AddUnique(object item)
		{
			if (!InnerList.Contains(item))
			{
				InnerList.Add(item);
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
			return InnerList.ToArray(targetType);
		}
		
		public void Sort()
		{
			InnerList.Sort();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();			
			for (int i=0; i<InnerList.Count; ++i)
			{
				if (i>0) { sb.Append(", "); }
				sb.Append(InnerList[i]);
			}
			return sb.ToString();
		}

		void InnerCollect(List target, Predicate condition)
		{
			foreach (object item in InnerList)
			{
				if (condition(item))
				{
					target.Add(item);
				}
			}
		}
	}
}
