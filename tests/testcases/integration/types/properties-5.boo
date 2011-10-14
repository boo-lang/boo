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
assert getter.IsStatic, "static property getter must be static"
