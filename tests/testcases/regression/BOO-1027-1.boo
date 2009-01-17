"""
foo
"""

class X:
	pass

abstract class A:
	virtual def foo(ref x as X):
		pass

class B (A):
	override def foo(ref x as X):
		print "foo"

x = X()
a as A = B()
a.foo(x)

