namespace BooCompiler.Tests

import System
import NUnit.Framework

static class Exceptions:
	public def Expecting[of T(Exception)](action as Action):
		try:
			action()
		except x as Exception:
			return if x isa T
			raise
		Assert.Fail("{0} expected!", typeof(T).Name)
