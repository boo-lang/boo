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

literal = ast:
	[once]
	def bar():
		return foo()

"""
literal = ast:
	class Foo:
		def bar():
			print 'Hello'
		end
	end
end

literal = ast:
	def main():
		print('Hello, world!')
	end
end

literal = ast:
	private foo as int
end

literal = ast:
	private foo = 0
end

literal = ast:
	private foo as object = object()
end

literal = ast:
	event Foo as Bar
end

literal = ast:
	[once]
	def bar():
		return foo()
	end
end

