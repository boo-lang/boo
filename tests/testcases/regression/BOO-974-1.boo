"""
foo
"""
class Foo:
	def Do(param as string):
		print "!!!"
	def Do[of T](param as T):
		print param
	def Test():
		Do[of string]('foo')

Foo().Test()
