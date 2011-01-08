"""
BCE0130-1.boo(9,13): BCE0130: 'partial' can only be applied to top level class, interface and enum definitions.
BCE0130-1.boo(11,13): BCE0130: 'partial' can only be applied to top level class, interface and enum definitions.
BCE0130-1.boo(14,17): BCE0130: 'partial' can only be applied to top level class, interface and enum definitions.
BCE0130-1.boo(17,19): BCE0130: 'partial' can only be applied to top level class, interface and enum definitions.
BCE0130-1.boo(20,16): BCE0130: 'partial' can only be applied to top level class, interface and enum definitions.
"""
partial class Foo:
	partial fld1 as int
	
	partial Property1:
		get: return 3
	
	partial def Method1():
		pass
		
	partial class Nested:
		pass

partial struct FooStruct:
	public Foo as int

partial enum FooEnum:
	Foo

partial interface IFoo:
	pass

