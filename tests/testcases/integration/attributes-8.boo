"""
DemoAttribute constructor
DemoAttribute
"""
import System

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple: false, Inherited:true)]
class DemoAttribute(Attribute):
	def constructor():
		print "DemoAttribute constructor"

class DemoClass:
	def DemoMethod([Demo] demoArg):
		pass
			
print typeof(DemoClass).GetMethod("DemoMethod").GetParameters()[0].GetCustomAttributes(true)[0]

