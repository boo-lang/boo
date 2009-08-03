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
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	public class Memo<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> _cachedValues;

		public Memo(IEqualityComparer<TKey> comparer)
		{
			_cachedValues = new Dictionary<TKey, TValue>(comparer);
		}

		public Memo()
		{
			//NB: SafeComparer is required to workaround a weird RuntimeMethodInfo.Equals bug
			//    when TKey is a MemberInfo on .NET 3.5 (not reproducible on 2.0, 4.0b1 and mono)
			_cachedValues = new Dictionary<TKey, TValue>(SafeComparer.Instance);
		}

		private Memo(IDictionary<TKey, TValue> cachedValues)
		{
			_cachedValues = cachedValues;
		}

		public Memo<TKey, TValue> Clone()
		{
			return new Memo<TKey, TValue>(new Dictionary<TKey, TValue>(_cachedValues));
		}

		public ICollection<TValue> Values
		{
			get { return _cachedValues.Values; }
		}

		public TValue Produce(TKey key, Func<TKey, TValue> producer)
		{
			TValue cachedValue;
			if (_cachedValues.TryGetValue(key, out cachedValue))
				return cachedValue;

			TValue newValue = producer(key);
			_cachedValues.Add(key, newValue);
			return newValue;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _cachedValues.TryGetValue(key, out value);
		}

		public void Add(TKey key, TValue value)
		{
			_cachedValues.Add(key, value);
		}

		private sealed class SafeComparer : IEqualityComparer<TKey>
		{
			public static SafeComparer Instance
			{
				get
				{
					if (null == instance)
						instance = new SafeComparer();
					return instance;
				}
			}
			static SafeComparer instance;

			public bool Equals(TKey x, TKey y)
			{
				if (null == x)
					return (null == y);
				return x.Equals(y);
			}

			public int GetHashCode(TKey obj)
			{
				if (null == obj)
					throw new ArgumentNullException("obj");
				return obj.GetHashCode();
			}
		}
	}
}

