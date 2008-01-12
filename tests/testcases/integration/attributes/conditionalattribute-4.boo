"""
something
"""
import System.Diagnostics

[Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
def PrintSomething[of T](x as T):
	print x

[Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
def PrintNothing[of T](x as T):
	print x

PrintSomething[of string]("something")
PrintNothing[of string]("nothing")

