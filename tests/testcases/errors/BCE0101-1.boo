"""
BCE0101-1.boo(12,16): BCE0101: The return type of a generator must be either 'System.Collections.IEnumerable' or 'object'.
"""
import System.Collections

def foo() as object:
	yield 0
	
def bar() as IEnumerable:
	yield 1
	
def error() as int:
	yield 3
