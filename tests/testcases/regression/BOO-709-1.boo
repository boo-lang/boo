import System

[Simple]
class TestClass:
	
	[Simple]
	public thingy = "Hello, World!"
	
	[Simple]
	event yeah as callable()
	
	[Simple]
	identity([Simple] value):
		get:
			return value	
	
	[Simple]
	def constructor([Simple] param):
		pass
	
	[Simple]
	def foo([Simple] param):
		pass
		
[Simple]
interface TestInterface:
	pass
	
[Simple]
enum TestEnum:
	[Simple] yo

class SimpleAttribute(System.Attribute):
	pass
	
def assertParameterAttribute(param as System.Reflection.ParameterInfo):
	attributes = param.GetCustomAttributes(false)
	assert SimpleAttribute in (obj.GetType() for obj in attributes), \
		"Simple attribute not found in '${param}'"
		
def assertAttribute(member as System.Reflection.MemberInfo):
	attributes = member.GetCustomAttributes(false)
	assert SimpleAttribute in (obj.GetType() for obj in attributes), \
		"Simple attribute not found in '${member}'"
	
assertAttribute(TestClass)
assertAttribute(typeof(TestClass).GetField("thingy"))
assertAttribute(m = typeof(TestClass).GetMethod("foo"))
assertParameterAttribute(m.GetParameters()[0])
assertAttribute(p = typeof(TestClass).GetProperty("identity"))
assertParameterAttribute(p.GetGetMethod().GetParameters()[0])
assertAttribute(typeof(TestClass).GetEvent("yeah"))
assertAttribute(c = typeof(TestClass).GetConstructors()[0])
assertParameterAttribute(c.GetParameters()[0])
assertAttribute(TestInterface)
assertAttribute(TestEnum)
assertAttribute(typeof(TestEnum).GetField("yo"))
