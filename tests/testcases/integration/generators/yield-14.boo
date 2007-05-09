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

g = Generators().onetwothree()
if Environment.Version > Version(1, 1, 0, 0):
	assert g.GetType().BaseType.Name.StartsWith("GenericGenerator")
else:
	assert g.GetType().BaseType == AbstractGenerator
assert "1 0 3" == join(g)
