"""
bar
"""
class Bar(Foo):
	X as string:
		get:
			return "bar"

class Foo:
	X as string:
		virtual get:
			return "foo"

print Bar().X
