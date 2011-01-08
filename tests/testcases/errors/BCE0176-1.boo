"""
BCE0176-1.boo(7,14): BCE0176: Incompatible partial definition for type 'Foo', expecting 'class' but got 'enum'.
"""
partial class Foo:
	pass
	
partial enum Foo:
	pass

