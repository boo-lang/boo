import NUnit.Framework from "nunit.framework"

class InstanceCount:
	
	static _instances = 0

	def constructor():
		++_instances
		
	static Instances:
		get:
			return _instances
		
type = InstanceCount
property = type.GetProperty("Instances")
getter = property.GetGetMethod()
Assert.IsTrue(getter.IsStatic, "static property getter must be static")
