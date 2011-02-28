"""
non-generic
42
generic
*42*
"""

import System.Collections.Generic

def Method[of T](arg as IList of T):
	print "generic"
	return arg[0]

def Method(arg as IList of int):
	print "non-generic"
	return arg[0]
		
print Method(List[of int]() { 42 })
print Method(List[of string]() { "*42*" })
