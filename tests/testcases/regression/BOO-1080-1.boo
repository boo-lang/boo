"""
Foo
"""

class A[of T]:
	internal def Foo():
		print "Foo"

A[of int]().Foo()

