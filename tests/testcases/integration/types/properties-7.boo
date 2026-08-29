class InstanceCount:
	
	[property(Instances)]
	static _instances = 0

	def constructor():
		++_instances
		
type = InstanceCount
property = type.GetProperty("Instances")
assert property.GetGetMethod().IsStatic, "static property getter must be static"
assert property.GetSetMethod().IsStatic, "static property setter must be static"
