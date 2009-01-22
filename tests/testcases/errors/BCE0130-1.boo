"""
BCE0130-1.boo(10,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(12,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(16,17): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(19,16): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(22,14): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(25,19): BCE0130: 'partial' can only be applied to class definitions.
"""
class Foo:
	partial fld1 as int
	
	partial Property1:
		get:
			return 3
	
	partial def Method1():
		pass

partial struct FooStruct:
	public Foo as int

partial enum FooEnum:
	Foo

partial interface IFoo:
	pass

