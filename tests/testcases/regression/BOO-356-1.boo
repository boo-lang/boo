"""
foo
bar
foo
"""
class Klass:
	func as callable

	def bar():
		print "bar"
		func = foo

	def foo():
		print "foo"
		func = bar

	def main():
		func = foo
		func()
		func()
		func()

obj = Klass()
obj.main()
