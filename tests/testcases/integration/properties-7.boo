import NUnit.Framework from "nunit.framework"

class InstanceCount:
	
	[property(Instances)]
	static _instances = 0

	def constructor():
		++_instances
		
type = InstanceCount
property = type.GetProperty("Instances")
Assert.IsTrue(property.GetGetMethod().IsStatic, "static property getter must be static")
Assert.IsTrue(property.GetSetMethod().IsStatic, "static property setter must be static")
