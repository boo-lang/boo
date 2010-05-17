"""
literal = [|
	class Foo:

		def bar():
			print 'Hello'
|]

literal = [|
	def main():
		print('Hello, world!')
|]

literal = [|
	private foo as int
|]

literal = [|
	private foo = 0
|]

literal = [|
	private foo as object = object()
|]

literal = [|
	event Foo as Bar
|]

literal = [|
	[once]
	def bar():
		return foo()
|]

literal = [|
	protected Property:
		get:
			return null
|]
"""
literal = [|
	class Foo:

		def bar():
			print 'Hello'
|]

literal = [|
	def main():
		print('Hello, world!')
|]

literal = [|
	private foo as int
|]

literal = [|
	private foo = 0
|]

literal = [|
	private foo as object = object()
|]

literal = [|
	event Foo as Bar
|]

literal = [|
	[once]
	def bar():
		return foo()
|]

literal = [|
	protected Property:
		get: return null
|]
