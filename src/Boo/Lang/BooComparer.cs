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

namespace Boo.Lang
{
	using System;
	using System.Collections;
	
	/// <summary>
	/// Compares items lexicographically through IEnumerable whenever
	/// they don't implement IComparable.
	/// </summary>
	public class BooComparer : IComparer
	{
		public static readonly IComparer Default = new BooComparer();
		
		private BooComparer()
		{
		}
		
		public int Compare(object lhs, object rhs)
		{
			if (null == lhs)
			{
				if (null == rhs)
				{
					return 0;
				}
				
				return -1;
			}
			else
			{
				if (null == rhs)
				{
					return 1;
				}
				
				IComparable lhsComparable = lhs as IComparable;
				if (null == lhsComparable)
				{
					IComparable rhsComparable = rhs as IComparable;
					if (null == rhsComparable)
					{
						IEnumerable lhsEnumerable = lhs as IEnumerable;
						IEnumerable rhsEnumerable = rhs as IEnumerable;
						if (null != lhsEnumerable && null != rhsEnumerable)
						{
							return CompareEnumerables(lhsEnumerable, rhsEnumerable);
						}
						throw new ArgumentException(ResourceManager.GetString("CantCompareItems"));
					}
					return -1*(rhsComparable.CompareTo(lhs));
				}
				return lhsComparable.CompareTo(rhs);
			}
		}
		
		int CompareEnumerables(IEnumerable lhs, IEnumerable rhs)
		{
			IEnumerator lhsEnum = lhs.GetEnumerator();
			IEnumerator rhsEnum = rhs.GetEnumerator();
			
			while (lhsEnum.MoveNext())
			{
				if (!rhsEnum.MoveNext())
				{
					return 1;
				}
				
				int value = Compare(lhsEnum.Current, rhsEnum.Current);
				if (0 == value)
				{
					continue;
				}
				return value;
			}
			
			if (rhsEnum.MoveNext())
			{
				return -1;
			}
			
			return 0;
		}
	}
}
