import System

class TestClass:
	[Simple]
	public thingy = "Hello, World!"

class SimpleAttribute(System.Attribute):
	pass
	
attributes = typeof(TestClass).GetField("thingy").GetCustomAttributes(false)
assert SimpleAttribute in (obj.GetType() for obj in attributes)
