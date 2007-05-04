import System
import System.Collections

class Generators:
	
	def onetwothree():
		yield 1
		yield
		yield 3
	
type = Generators
method = type.GetMethod("onetwothree")
assert method is not null

returnType = method.ReturnType
assert IEnumerable in returnType.GetInterfaces()

assert "1 0 3" == join(Generators().onetwothree())
