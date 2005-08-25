"""
BCW0001-3.boo(8,11): BCW0001: WARNING: Type 'Foo' does not provide an implementation for 'AFoo.Bar()' and will be marked abstract
"""
abstract class AFoo:
	abstract def Bar():
		pass
		
class Foo(AFoo):
	pass
