using System;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
	public class MemoizedFunction<TArg, TResult>
	{
		private readonly Func<TArg, TResult> _function;
		private readonly IDictionary<TArg, TResult> _cachedValues;

		public MemoizedFunction(IEqualityComparer<TArg> comparer, Func<TArg, TResult> function)
		{
			_function = function;
			_cachedValues = new Dictionary<TArg, TResult>(comparer);
		}

		public MemoizedFunction(Func<TArg, TResult> function) : this(SafeComparer.Instance, function)
		{
			//NB: SafeComparer is required to workaround a weird RuntimeMethodInfo.Equals bug
			//    when TKey is a MemberInfo on .NET 3.5 (not reproducible on 2.0, 4.0b1 and mono)
		}

		private MemoizedFunction(IDictionary<TArg, TResult> cachedValues, Func<TArg, TResult> function)
		{
			_cachedValues = cachedValues;
			_function = function;
		}

		public MemoizedFunction<TArg, TResult> Clone()
		{
			return new MemoizedFunction<TArg, TResult>(new Dictionary<TArg, TResult>(_cachedValues), _function);
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

		private sealed class SafeComparer : IEqualityComparer<TArg>
		{
			public static SafeComparer Instance
			{
				get { return instance ?? (instance = new SafeComparer()); }
			}

			static SafeComparer instance;

			public bool Equals(TArg x, TArg y)
			{
				return object.Equals(x, y);
			}

			public int GetHashCode(TArg obj)
			{
				if (null == obj)
					throw new ArgumentNullException("obj");
				return obj.GetHashCode();
			}
		}
	}

	public class MemoizedFunction<T1, T2, TResult>
	{	
		Dictionary<T1, Dictionary<T2, TResult>> _cache = new Dictionary<T1, Dictionary<T2, TResult>>();
		Func<T1, T2, TResult> _func;

		public MemoizedFunction(Func<T1, T2, TResult> func)
		{
			_func = func;
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
