"""
BCW0011-6.boo(7,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo.Bar[of T]()', a stub has been created.
"""
interface IFoo:
	def Bar[of T]()

class Foo(IFoo):
	pass

Foo().Bar[of int]()

