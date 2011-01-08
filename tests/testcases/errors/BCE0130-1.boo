"""
BCE0130-1.boo(9,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(11,13): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(15,17): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(18,16): BCE0130: 'partial' can only be applied to class definitions.
BCE0130-1.boo(24,19): BCE0130: 'partial' can only be applied to class definitions.
"""
partial class Foo:
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

