"""
BCW0001-2.boo(7,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo.Bar()', a stub has been created.
"""
interface IFoo:
	def Bar()
		
class Foo(IFoo):
	pass
