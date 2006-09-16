import System.Collections.Generic

class Foo:
	def bar() as List of int:
		pass
	

assert typeof(List of int) is typeof(Foo).GetMethod("bar").ReturnType
