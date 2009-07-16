"""
FOO
"""

class A:
	b = B[of int]()

	def AssertFoo():
		assert 42 == b.Foo()

class B[of T]():
	def Foo():
		print "FOO"
		return 42

A().AssertFoo()

