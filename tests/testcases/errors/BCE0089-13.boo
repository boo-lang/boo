"""
BCE0089-13.boo(15,5): BCE0089: Type 'Foo' already has a definition for 'bar()'.
"""
import Boo.Lang.Compiler

class Foo:

	def bar():
		pass
		
	def bar(value):
		pass
		
[Extension]
def bar(f as Foo):
	pass
