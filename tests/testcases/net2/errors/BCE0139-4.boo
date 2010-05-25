"""
BCE0139-4.boo(8,7): BCE0139: 'Foo[of T]' requires '1' arguments.
"""
static class Foo[of T]:
	def Bar(value as T):
		pass

bar = Foo.Bar

