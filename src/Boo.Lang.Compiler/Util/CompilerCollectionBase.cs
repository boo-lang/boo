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

using System.Collections.Generic;
using Boo.Lang.Runtime;

namespace Boo.Lang.Compiler.Util
{
	using System;
	using System.Collections;
	
	public class CompilerCollectionBase<T> : ICollection<T>, ICollection
	{
		private readonly List<T> _items = new List<T>();

		public CompilerCollectionBase()
		{	
		}

		public T this[int index]
		{
			get
			{
				return index < 0
				       	? _items[RuntimeServices.NormalizeIndex(_items.Count, index)]
				       	: _items[index];
			}
		}
		
		#region ICollection Members
		
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return this; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_items).CopyTo(array, index);
		}

		#endregion

		public int Count
		{
			get { return _items.Count; }
		}

		#region ICollection<T> Members

		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		public void Add(T item)
		{
			if (null == item) throw new ArgumentNullException("item");
			_items.Add(item);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return _items.Remove(item);
		}

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		public IEnumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		#endregion
	}
}
