"""
BCE0089-2.boo(7,5): BCE0089: Type 'Foo' already has a definition for 'bar'.
"""
class Foo:
	bar as object
	
	bar = null # still a field declaration
