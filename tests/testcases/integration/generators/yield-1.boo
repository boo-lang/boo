import System
import System.Collections

class Generators:
	
	def onetwothree():
		yield 1
		yield 2
		yield 3
	
type = Generators
method = type.GetMethod("onetwothree")
assert method is not null

returnType = method.ReturnType
assert IEnumerable in returnType.GetInterfaces()

