"""
BCW0001-8.boo(10,11): BCW0001: Type 'Foo' does not provide an implementation for 'IFoo.Bar' and will be marked abstract
"""
interface IFoo:
	def Bar()
		
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	pass
