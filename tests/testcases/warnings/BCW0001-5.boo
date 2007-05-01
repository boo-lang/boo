"""
BCW0001-5.boo(7,12): BCW0011: WARNING: Type 'Foo1' does not provide an implementation for 'IFoo.Bar()', a stub has been created.
"""
interface IFoo:
	def Bar()
		
class Foo1(IFoo):
	pass
	
class Foo2(Foo1):
	pass
