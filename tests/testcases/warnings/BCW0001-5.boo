"""
BCW0001-5.boo(8,12): BCW0001: WARNING: Type 'Foo1' does not provide an implementation for 'IFoo.Bar' and will be marked abstract
BCW0001-5.boo(11,12): BCW0001: WARNING: Type 'Foo2' does not provide an implementation for 'Foo1.Bar' and will be marked abstract
"""
interface IFoo:
	def Bar()
		
class Foo1(IFoo):
	pass
	
class Foo2(Foo1):
	pass
