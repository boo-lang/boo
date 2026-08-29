"""
BCE0137-3.boo(11,7): BCE0137: Property 'Foo.Baz' is write only.
"""
class Foo:
	Baz[index] as string:
		set:
			print index, value

foo = Foo()
foo.Baz[42] = "baz"
print foo.Baz[42]
