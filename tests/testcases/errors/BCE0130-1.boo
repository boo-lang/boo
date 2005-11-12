"""
BCE0130-1.boo(7,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(9,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(13,17): BCE0130: 'partial' can only be applied to class definitions.
"""
class Foo:
	partial fld1 as int
	
	partial Property1:
		get:
			return 3
	
	partial def Method1():
		pass
