"""
BCW0011-7.boo(7,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo.Bar[of T](T)', a stub has been created.
"""
interface IFoo:
	def Bar[of T](x as T) as T

class Foo(IFoo):
	pass

foo as IFoo = Foo()
foo.Bar[of int](42)

