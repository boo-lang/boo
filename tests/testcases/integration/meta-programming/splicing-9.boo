"""
(o as Foo.Bar)
(o as Foo.Bar)
(o as (Foo.Bar))
"""
class Foo:
	class Bar:
		pass
		
typeRef = Foo.Bar
print([| o as $typeRef |].ToCodeString())

# strings get lifted to simple type references too
typeName = "Foo.Bar"
print([| o as $typeName |].ToCodeString())

# to create an array type from an string
print([| o as ($typeName) |].ToCodeString())
