"""
BCE0107-1.boo(5,9): BCE0107: Value types cannot declare parameterless constructors.
"""
class Foo(System.ValueType):
	def constructor():
		pass
