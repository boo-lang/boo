namespace Statsia.Lang.Compiler

import System
import System.Collections.Generic
import System.Linq.Enumerable

// Some helper methods to use with transformations
public static class TransformHelpers:
	
	
	[Extension]	
	public def Map[of TSource, TResult](source as (TSource), mapper as Func[of TSource, TResult]) as (TResult):
		result = array(TResult, source.Length)
		for i, x in enumerate(source):
			result[i] = mapper(x)
		return result
	
	[Extension]
	public def Apply[of TSource, TResult](source as TSource, mapper as Func[of TSource, TResult]) as TResult:
		return mapper(source)