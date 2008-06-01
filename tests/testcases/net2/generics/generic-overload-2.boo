"""
generic
42
"""

def Method[of T](arg as T):
	print "generic"
	return arg

def Method(arg as object):
	print "non-generic"
	return arg
	
print Method(42)
