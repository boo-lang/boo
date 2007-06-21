"""
BCW0001-1.boo(8,11): BCW0001: WARNING: Type 'Foo' does not provide an implementation for 'IFoo.Bar' and will be marked abstract.
"""
interface IFoo:
	Bar:
		get
		
class Foo(IFoo):
	pass
