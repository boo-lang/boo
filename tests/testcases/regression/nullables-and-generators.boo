class Foo:
	cycle as int?
	
	def generator():
		cycle = null
		yield cycle
		
i, = Foo().generator()
assert i == null
