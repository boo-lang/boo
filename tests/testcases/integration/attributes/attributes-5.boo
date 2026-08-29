import System

// Attribute resolution should be such that
// FooAttribute is chosen over Foo

class FooAttribute(Attribute):

	[property(Bar)] _bar = ""
	
class Foo(Attribute):	
	pass
	
[Foo(Bar: "Attention please!")]
class Baz:
	pass
	
attribute as FooAttribute = Attribute.GetCustomAttribute(Baz, FooAttribute)
assert attribute is not null
assert "Attention please!" == attribute.Bar
	
