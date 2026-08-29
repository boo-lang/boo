"""
BCE0137-2.boo(15,11): BCE0137: Property 'Foo.Baz' is write only.
"""
class Bar:
	TheFoo as Foo:
		get:
			return Foo()
			
class Foo:
	Baz as string:
		set:
			print value

bar = Bar()
print bar.TheFoo.Baz
bar.TheFoo.Baz = "baz"
