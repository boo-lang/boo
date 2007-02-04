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
assert returnType.IsClass
# assert returnType.BaseType is AbstractGenerator
assert IEnumerable in returnType.GetInterfaces()

# attribute as EnumeratorItemTypeAttribute
# attribute = Attribute.GetCustomAttribute(returnType, EnumeratorItemTypeAttribute)
# assert attribute is not null
# assert attribute.ItemType is int

assert "1 0 3" == join(Generators().onetwothree())
