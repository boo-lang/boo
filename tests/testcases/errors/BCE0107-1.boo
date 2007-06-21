"""
BCE0107-1.boo(5,9): BCE0107: Value types cannot declare parameter-less constructors.
"""
class Foo(System.ValueType):
	def constructor():
		pass
