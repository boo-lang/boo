"""
BCW0001-2.boo(7,11): BCW0001: Type 'Foo' does not provide an implementation for 'IFoo.Bar' and will be marked abstract
"""
interface IFoo:
	def Bar()
		
class Foo(IFoo):
	pass
