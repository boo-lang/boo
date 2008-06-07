"""
BCW0011-8.boo(8,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'AbstractFoo.Bar[of T](T)', a stub has been created.
"""
abstract class AbstractFoo:
	abstract def Bar[of T](x as T) as T:
		pass

class Foo(AbstractFoo):
	pass

Foo().Bar[of int](42)

