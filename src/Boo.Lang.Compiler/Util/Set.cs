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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	public class Set<T> : ICollection<T>
	{
		private readonly Dictionary<T, bool> _elements = new Dictionary<T, bool>();

		public Set()
		{	
		}

		public Set(IEnumerable<T> elements)
		{
			foreach (T element in elements) Add(element);
		}

		public void Add(T element)
		{
			_elements[element] = true;
		}

		public void Clear()
		{
			_elements.Clear();
		}

		public bool Contains(T element)
		{
			bool value;
			return _elements.TryGetValue(element, out value);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_elements.Keys.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _elements.Count; }
		}

		#region Implementation of IEnumerable
		public IEnumerator<T> GetEnumerator()
		{
			return _elements.Keys.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		#endregion

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T element)
		{
			return _elements.Remove(element);
		}

		public void RemoveAll(Predicate<T> predicate)
		{
			List<T> toRemove = new List<T>();
			foreach (T element in _elements.Keys)
				if (predicate(element))
					toRemove.Add(element);
			foreach (T element in toRemove)
				Remove(element);
		}

		public override string ToString()
		{
			return "{" + Builtins.join(this) + "}";
		}

		public bool ContainsAll(IEnumerable<T> elements)
		{
			foreach (T element in elements)
				if (!Contains(element))
					return false;
			return true;
		}

		public T[] ToArray()
		{
			T[] result = new T[Count];
			CopyTo(result, 0);
			return result;
		}
	}
}
