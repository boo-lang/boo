"""
42
foo
"""

abstract class AbstractFoo:
	abstract def Bar[of T](x as T) as T:
		pass

class Foo(AbstractFoo):
	def Bar[of T](x as T) as T:
		print x
		return x

foo = Foo()
assert 42 == foo.Bar[of int](42)
assert "foo" == foo.Bar[of string]("foo")

