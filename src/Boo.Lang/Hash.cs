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
using System.Runtime.Serialization;
using Boo.Lang.Runtime;

namespace Boo.Lang
{
	/// <summary>
	/// Hash.
	/// </summary>
	[Serializable]
	[EnumeratorItemType(typeof(DictionaryEntry))]
	public class Hash : Hashtable, IEquatable<Hash>
	{
		public Hash() : base(BooHashCodeProvider.Default)
		{
		}

		public Hash(IDictionary other) : this()
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}
			foreach (DictionaryEntry entry in other)
			{
				Add(entry.Key, entry.Value);
			}
		}

		public Hash(IEnumerable enumerable) : this()
		{
			if (null == enumerable)
			{
				throw new ArgumentNullException("enumerable");
			}

			foreach (Array tuple in enumerable)
			{
				Add(tuple.GetValue(0), tuple.GetValue(1));
			}
		}

		public Hash(bool caseInsensitive) : base(StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public Hash(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		override public object Clone()
		{
			return new Hash(this);
		}

		override public bool Equals(object other)
		{
			if (null == other) return false;
			if (this == other) return true;

			Hash hash = other as Hash;
			return Equals(hash);
		}

		public bool Equals(Hash other)
		{
			if (null == other) return false;
			if (this == other) return true;
			if (Count != other.Count) return false;

			foreach (DictionaryEntry entry in other)
			{
				if (!ContainsKey(entry.Key)) return false;
				if (!RuntimeServices.EqualityOperator(entry.Value, this[entry.Key])) return false;
			}
			return true;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			
			foreach (object item in this)
			{
				hashCode ^= GetHash(item);
			}
			
			return hashCode;
		}
	}
}
