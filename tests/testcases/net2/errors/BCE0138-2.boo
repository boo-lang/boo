"""
BCE0138-2.boo(11,7): BCE0138: 'BCE0138-2Module.Method' is not a generic definition.
"""

def Method(arg as int):
	print "Method(int)"

def Method(arg as bool):
	print "Method(bool)"

Method[of int](3)
