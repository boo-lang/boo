"""
Method[of T]
"""

def Method(arg as int):
	print "Method"

def Method[of T](arg as T):
	print "Method[of T]"

def Method[of U](arg as U, otherArg as bool):
	print "Method[of U]"

def Method[of T, U](arg as T):
	print "Method[of T, U]"

Method[of int](3)
