"""
-1
"""
import System

class FooAttribute(Attribute):
	public Value as int
	
class Bar:
	[Foo(Value: -1)]
	def Zeng():
		pass
		
def DumpFooValue(methodName as string):		
	attributes = Attribute.GetCustomAttributes(typeof(Bar).GetMethod(methodName), FooAttribute)
	assert 1 == len(attributes)
	print((attributes[0] as FooAttribute).Value)
	
DumpFooValue("Zeng")
