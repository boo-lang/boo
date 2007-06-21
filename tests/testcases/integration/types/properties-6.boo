import NUnit.Framework from "nunit.framework"

class InstanceCount:
	
	[getter(Instances)]
	static _instances = 0

	def constructor():
		++_instances
		
type = InstanceCount
property = type.GetProperty("Instances")
getter = property.GetGetMethod()
Assert.IsTrue(getter.IsStatic, "static property getter must be static")
Assert.IsNull(property.GetSetMethod(), "setter must be null")
