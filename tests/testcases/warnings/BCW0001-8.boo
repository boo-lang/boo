"""
BCW0001-8.boo(10,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'IFoo.Bar()', a stub has been created.
"""
interface IFoo:
	def Bar()
		
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	pass
