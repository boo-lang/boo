"""
BCE0017-4.boo(18,4): BCE0017: The best overload for the method 'BCE0017_4Module.Foo(System.Collections.IEnumerable)' is not compatible with the argument list '(Test)'.
"""

import System.Collections

macro enableStrict:
	Parameters.Strict = true
enableStrict


class Test:
	pass

def Foo(x as IEnumerable):
	pass

Foo(Test())

