namespace Statsia.Lang.Compiler
import System
import System.Collections.Generic

// Some helper methods to use with transformations
public static class TransformHelpers:
	
	public def InfiniteEnumerator[of T](value as T) as IEnumerator[of T]:
		yield value