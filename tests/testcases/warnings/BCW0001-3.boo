"""
BCW0001-1.boo(7,11): BCW0001: Type 'Foo' does not provide an implementation for 'AFoo.Bar' and will be marked abstract
"""
abstract class AFoo:
	abstract def Bar():
		pass
		
class Foo(AFoo):
	pass
