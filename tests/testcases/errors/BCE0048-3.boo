"""
BCE0048-3.boo(8,20): BCE0048: Type 'Console.WriteLine' does not support slicing.
"""
import System

# Nothing to do with the builtins: any method group reaches the same place.
def Read():
	return Console.WriteLine[0]
