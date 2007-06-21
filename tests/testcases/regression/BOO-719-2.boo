"""
bar
"""
class Bar(Foo):
	def X():
		return "bar"

class Foo:
	virtual def X():
		return "foo"

print Bar().X()
