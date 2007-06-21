"""
BCW0001-3.boo(8,11): BCW0011: WARNING: Type 'Foo' does not provide an implementation for 'AFoo.Bar()', a stub has been created.
"""
abstract class AFoo:
	abstract def Bar():
		pass
		
class Foo(AFoo):
	pass
