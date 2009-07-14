#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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

using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	/// <summary>
	/// Compares arrays based on their items.
	/// </summary>
	internal sealed class ArrayEqualityComparer<T>: IEqualityComparer<T[]>
	{
		private static ArrayEqualityComparer<T> _default = null;

		private ArrayEqualityComparer()
		{
		}

		public static ArrayEqualityComparer<T> Default
		{
			get { return _default ?? (_default = new ArrayEqualityComparer<T>()); }
		}

		public bool Equals(T[] x, T[] y)
		{
			// Null equals null, and nothing else
			if (x == null && y == null) return true;
			if (x == null || y == null) return false;
			
			// Compare arrays' lengths
			if (x.Length != y.Length) return false;
			
			// Compare arrays' contents
			for (int i = 0; i < x.Length; i++)
			{
				if ((x[i] == null && y[i] != null) || (!x[i].Equals(y[i])))
				{
					return false;
				}
			}
			
			return true;
		}
		
		public int GetHashCode(T[] args)
		{
			// Make a simple hash code from the hash codes of the items
			int hash = 0;
			for (int i = 0; i < args.Length; i++)
			{
				hash ^= i ^ args[i].GetHashCode();
			}
			
			return hash;
		}
	}
}

