#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
