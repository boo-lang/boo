"""
(o as Foo.Bar)
"""
class Foo:
	class Bar:
		pass
		
typeRef = Foo.Bar
print([| o as $typeRef |].ToCodeString())
