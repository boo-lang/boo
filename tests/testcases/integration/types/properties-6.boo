class InstanceCount:
	
	[getter(Instances)]
	static _instances = 0

	def constructor():
		++_instances
		
type = InstanceCount
property = type.GetProperty("Instances")
getter = property.GetGetMethod()
assert getter.IsStatic, "static property getter must be static"
assert property.GetSetMethod() is null
