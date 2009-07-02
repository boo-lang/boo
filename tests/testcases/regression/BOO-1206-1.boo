"""
42
"""

class Foo[of T(struct)]:
	internal def fun(ref q as T):
		print q

class Bar[of T(struct)]:
	val as T

	def constructor(val as T):
		.val = val

	internal def fun2(ex as Foo[of T]):
		ex.fun(val)


foo = Foo[of int]()
Bar[of int](42).fun2(foo)
