"""
something
"""
import System.Diagnostics

[Conditional("BOO_COMPILER_TESTS_DEFINED_CONDITIONAL")]
def PrintSomething():
	print "something"

[Conditional("BOO_COMPILER_TESTS_NOT_DEFINED_CONDITIONAL")]
def PrintNothing():
	print "nothing"

PrintSomething()
PrintNothing()

