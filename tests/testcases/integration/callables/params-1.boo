import System
import System.Reflection

class Test:
	def constructor(*args):
		pass
		
	def Foo(*args):
		pass
		
	def Bar(*args as (string)):
		pass
		
callable Baz(*args)
		
def assertParams(method as MethodBase, parameterType as Type):
	assert method is not null
	assert 1 == len(method.GetParameters())

	param = method.GetParameters()[0]
	assert param.ParameterType is parameterType
	assert param.Name == "args"
	assert Attribute.GetCustomAttribute(param, ParamArrayAttribute) is not null
		
type = Test
assertParams(type.GetConstructors()[0], typeof((object)))
assertParams(type.GetMethod("Foo"), typeof((object)))
assertParams(type.GetMethod("Bar"), typeof((string)))

assertParams(typeof(Baz).GetMethod("Invoke"), typeof((object)))
