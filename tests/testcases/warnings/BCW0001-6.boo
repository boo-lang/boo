"""
BCW0001-6.boo(9,11): BCW0001: WARNING: Type 'Foo' does not provide an implementation for 'AFoo.Bar' and will be marked abstract.
"""
abstract class AFoo:
	abstract Bar:
		get:
			pass
		
class Foo(AFoo):
	pass
