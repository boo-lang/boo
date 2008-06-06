public interface GenericInterface[of T]:
	Property as T: 
		get
		set
	
	def Method(argument as T) as T
	
t = typeof(GenericInterface of int)
assert t.GetMethod("Method").ReturnType == typeof(int)
assert t.GetProperty("Property").PropertyType == typeof(int)
assert t.GetProperty("Property").GetGetMethod().ReturnType == typeof(int)

