#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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
	public class MemoizedFunction<TArg, TResult>
	{
		private readonly IEqualityComparer<TArg> _comparer;
		private readonly Func<TArg, TResult> _function;
		private readonly IDictionary<TArg, TResult> _cachedValues;

		public MemoizedFunction(Func<TArg, TResult> function)
			: this(SafeComparer<TArg>.Instance, function)
		{
			//NB: SafeComparer is required to workaround a weird RuntimeMethodInfo.Equals bug
			//	  when TKey is a MemberInfo on .NET 3.5 (not reproducible on 2.0, 4.0b1 and mono)
		}

		public MemoizedFunction(IEqualityComparer<TArg> comparer, Func<TArg, TResult> function)
			: this(comparer, function, new Dictionary<TArg, TResult>(comparer))
		{
		}

		private MemoizedFunction(IEqualityComparer<TArg> comparer, Func<TArg, TResult> function, IDictionary<TArg, TResult> cachedValues)
		{
			_cachedValues = cachedValues;
			_function = function;
			_comparer = comparer;
		}

		public MemoizedFunction<TArg, TResult> Clone()
		{
			return new MemoizedFunction<TArg, TResult>(_comparer, _function, new Dictionary<TArg, TResult>(_cachedValues, _comparer));
		}

		public ICollection<TResult> Values
		{
			get { return _cachedValues.Values; }
		}

		public TResult Invoke(TArg arg)
		{
			TResult cachedResult;
			if (_cachedValues.TryGetValue(arg, out cachedResult))
				return cachedResult;

			TResult newResult = _function(arg);
			_cachedValues.Add(arg, newResult);
			return newResult;
		}

		public void Clear(TArg arg)
		{
			_cachedValues.Remove(arg);
		}

		public void Clear()
		{
			_cachedValues.Clear();
		}

		public bool TryGetValue(TArg arg, out TResult result)
		{
			return _cachedValues.TryGetValue(arg, out result);
		}

		public void Add(TArg arg, TResult result)
		{
			_cachedValues.Add(arg, result);
		}
	}

	sealed class SafeComparer<T> : IEqualityComparer<T>
	{
		public static SafeComparer<T> Instance
		{
			get { return _instance ?? (_instance = new SafeComparer<T>()); }
		}

		private static SafeComparer<T> _instance;

		public bool Equals(T x, T y)
		{
			return object.Equals(x, y);
		}

		public int GetHashCode(T obj)
		{
			return obj.GetHashCode();
		}
	}

	public class MemoizedFunction<T1, T2, TResult>
	{
		readonly Dictionary<T1, Dictionary<T2, TResult>> _cache;
		readonly Func<T1, T2, TResult> _func;
		readonly IEqualityComparer<T1> _comparer;

		public MemoizedFunction(Func<T1, T2, TResult> func)
			: this(SafeComparer<T1>.Instance, func)
		{
		}

		public MemoizedFunction(IEqualityComparer<T1> comparer, Func<T1, T2, TResult> func)
			: this(comparer, func, new Dictionary<T1, Dictionary<T2, TResult>>(comparer))
		{
		}

		public MemoizedFunction(IEqualityComparer<T1> comparer, Func<T1, T2, TResult> func, Dictionary<T1, Dictionary<T2, TResult>> cache)
		{
			_comparer = comparer;
			_func = func;
			_cache = cache;
		}

		public MemoizedFunction<T1, T2, TResult> Clone()
		{
			return new MemoizedFunction<T1, T2, TResult>(
				_comparer,
				_func,
				new Dictionary<T1, Dictionary<T2, TResult>>(_cache, _comparer));
		}

		public void Clear(T1 arg1)
		{
			_cache.Remove(arg1);
		}

		public void Clear()
		{
			_cache.Clear();
		}

		public TResult Invoke(T1 arg1, T2 arg2)
		{
			Dictionary<T2, TResult> resultByArg2;
			if (_cache.TryGetValue(arg1, out resultByArg2))
			{
				TResult cached;
				if (resultByArg2.TryGetValue(arg2, out cached))
					return cached;
			}

			var result = _func(arg1, arg2);

			if (resultByArg2 == null)
			{
				resultByArg2 = new Dictionary<T2, TResult>();
				_cache.Add(arg1, resultByArg2);
			}
			resultByArg2.Add(arg2, result);

			return result;
		}
	}
}
