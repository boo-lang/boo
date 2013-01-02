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
using System.Collections;
using System.Collections.Generic;
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	/// <summary>
	/// Hash.
	/// </summary>
	[Serializable]
	[EnumeratorItemType(typeof(DictionaryEntry))]
	public class Hash : IDictionary, IEquatable<Hash>
#if !NO_ICLONEABLE
		, ICloneable
#endif
	{
		private readonly Dictionary<object, object> _dictionary;

		public Hash() : this(false)
		{
		}

		public Hash(bool caseInsensitive)
			: this(caseInsensitive ? BooEqualityComparers.CaseInsensitive : BooEqualityComparers.Default)
		{
		}

		public Hash(IEqualityComparer<object> comparer)
		{
			_dictionary = new Dictionary<object, object>(comparer);
		}

		public Hash(IDictionary other) : this()
		{
			AddAll(other);
		}

		private void AddAll(IDictionary other)
		{
			if (other == null)
				throw new ArgumentNullException("other");
			var enumerator = other.GetEnumerator();
			while (enumerator.MoveNext())
				Add(enumerator.Key, enumerator.Value);
		}

		public Hash(IEnumerable enumerable) : this()
		{
			if (null == enumerable)
				throw new ArgumentNullException("enumerable");

			foreach (Array tuple in enumerable)
				Add(tuple.GetValue(0), tuple.GetValue(1));
		}

		private Hash(IEqualityComparer<object> comparer, IDictionary dictionary) : this(comparer)
		{
			AddAll(dictionary);
		}

		[Obsolete("Prefer ShallowCopy method.")]
		public object Clone()
		{
			return ShallowCopy();
		}

		public Hash ShallowCopy()
		{
			return new Hash(_dictionary.Comparer, _dictionary);
		}

		public override bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			var hash = other as Hash;
			return Equals(hash);
		}

		public bool Equals(Hash other)
		{
			if (other == null) return false;
			if (ReferenceEquals(other, this)) return true;
			if (Count != other.Count) return false;

			foreach (var entry in other)
			{
				if (!ContainsKey(entry.Key)) return false;
				if (!RuntimeServices.EqualityOperator(entry.Value, this[entry.Key])) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			var hashCode = 0;

			foreach (var entry in _dictionary)
			{
				hashCode ^= Comparer.GetHashCode(entry.Key);
				hashCode ^= Comparer.GetHashCode(entry.Value);
			}

			return hashCode;
		}

		private IEqualityComparer<object> Comparer
		{
			get { return _dictionary.Comparer; }
		}

		public bool Contains(object key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool ContainsKey(object key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool ContainsValue(object value)
		{
			return _dictionary.ContainsValue(value);
		}

		public void Add(object key, object value)
		{
			_dictionary.Add(key, value);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public IEnumerator<DictionaryEntry> GetEnumerator()
		{
			foreach (var entry in _dictionary)
				yield return new DictionaryEntry(entry.Key, entry.Value);
		}
	
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		public void Remove(object key)
		{
			_dictionary.Remove(key);
		}

		public object this[object key]
		{
			get
			{
				object value;
				return _dictionary.TryGetValue(key, out value) ? value : null;
			}
			set { _dictionary[key] = value; }
		}

		public ICollection Keys
		{
			get { return _dictionary.Keys; }
		}

		public ICollection Values
		{
			get { return _dictionary.Values; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_dictionary).CopyTo(array, index);
		}

		public int Count
		{
			get { return _dictionary.Count; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}
	}
}