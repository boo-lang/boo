"""
BCE0101-1.boo(12,16): BCE0101: Return type 'int' cannot be used on a generator. Did you mean 'int*' ? Or use a 'System.Collections.IEnumerable' or 'object'.
"""
import System.Collections

def foo() as object:
	yield 0
	
def bar() as IEnumerable:
	yield 1
	
def error() as int:
	yield 3
