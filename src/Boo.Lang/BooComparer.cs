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

namespace Boo.Lang
{
	using System;
	using System.Collections;

	/// <summary>
	/// Compares items lexicographically through IEnumerable whenever
	/// they don't implement IComparable.
	/// </summary>
	[Serializable]
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
						//throw new ArgumentException(ResourceManager.GetString("CantCompareItems"));
						return lhs.Equals(rhs) ? 0 : 1;
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
