"""
literal = ast:
	class Foo:

		def bar():
			print 'Hello'

literal = ast:
	def main():
		print('Hello, world!')

literal = ast:
	private foo as int

literal = ast:
	private foo = 0

literal = ast:
	private foo as object = object()

literal = ast:
	event Foo as Bar

litera = ast:
	[once]
	def bar():
		return foo()

"""
literal = ast:
	class Foo:
		def bar():
			print 'Hello'
			
literal = ast:
	def main():
		print('Hello, world!')
		
literal = ast:
	private foo as int
	
literal = ast:
	private foo = 0
	
literal = ast:
	private foo as object = object()
	
literal = ast:
	event Foo as Bar
	
litera = ast:
	[once]
	def bar():
		return foo()
