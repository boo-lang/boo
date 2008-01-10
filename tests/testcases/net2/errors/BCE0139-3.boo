"""
BCE0139-3.boo(14,7): BCE0139: 'BCE0139-3Module.Method`2' requires '2' arguments.
"""

def Method(arg as int):
	print "Method(int)"

def Method[of T, U](arg as T):
	print "Method[of T, U](T)"

def Method[of T,U,V](arg as T):
	print "Method[of T,U,V](T)"

Method[of int](3)
