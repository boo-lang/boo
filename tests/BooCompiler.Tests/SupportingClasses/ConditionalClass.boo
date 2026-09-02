namespace BooCompiler.Tests.SupportingClasses

import System

class ConditionalClass:
	[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
	public static def PrintNothing(i as int):
		Console.WriteLine(i)

	[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
	public static def PrintSomething(s as string):
		Console.WriteLine(s)

	[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
	public static def PrintNoT[of T](s as T):
		Console.WriteLine(s)

	[System.Diagnostics.Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
	public static def PrintSomeT[of T](s as T):
		Console.WriteLine(s)
