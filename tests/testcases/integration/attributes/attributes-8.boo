"""
DemoAttribute constructor
DemoAttribute
DemoAttribute constructor
DemoAttribute
"""
import System

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple: false, Inherited:true)]
class DemoAttribute(Attribute):
	def constructor():
		print "DemoAttribute constructor"

class DemoClass:
	def constructor([Demo] arg1):
		pass
	def DemoMethod([Demo] demoArg) [Demo] as int:
		pass
			
print typeof(DemoClass).GetMethod("DemoMethod").GetParameters()[0].GetCustomAttributes(true)[0]
print typeof(DemoClass).GetConstructor((object,)).GetParameters()[0].GetCustomAttributes(true)[0]

//uncomment when DefineParameter(0...) bug is not around
//print typeof(DemoClass).GetMethod("DemoMethod").ReturnTypeCustomAttributes.GetCustomAttributes(true)[0]

