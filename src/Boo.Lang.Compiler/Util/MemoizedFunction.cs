using System;
using System.Collections.Generic;

namespace Boo.Lang.Compiler.Util
{
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
