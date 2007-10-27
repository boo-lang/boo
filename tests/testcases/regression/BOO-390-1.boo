"""
System.Diagnostics.DebuggableAttribute
System.Runtime.CompilerServices.RuntimeCompatibilityAttribute
FooAttribute
"""
import System

class FooAttribute(Attribute):
	pass
	
class Foo:
	pass
	
for attr in System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(false):
	print attr

[assembly: Foo()]
