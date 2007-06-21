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

litera = [|
	[once]
	def bar():
		return foo()
|]
"""
literal = [|
	class Foo:
		def bar():
			print 'Hello'
		end
	end
|]

literal = [|
	def main():
		print('Hello, world!')
	end
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

litera = [|
	[once]
	def bar():
		return foo()
	end
|]
