namespace BooCompiler.Tests.SupportingClasses

import System

class ObsoleteClass:
	[Obsolete("It is.")]
	public static Bar = 42

	[Obsolete("Indeed it is.")]
	public static def Foo():
		pass

	[Obsolete("We said so.")]
	public static Baz as int:
		get: return 42
