"""
42
foo
"""

interface IFoo:
	def Foo[of T](x as T) as T

class FooImpl(IFoo):
	def Foo[of T](x as T) as T:
		print x
		return x

foo as IFoo = FooImpl()
assert 42 == foo.Foo[of int](42)
assert "foo" == foo.Foo[of string]("foo")

