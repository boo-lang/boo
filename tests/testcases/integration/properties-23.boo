class Entity:
	abstract Name as string:
		get:
			pass
			
type = Entity
assert type.IsAbstract

property=type.GetProperty("Name")
assert property is not null
assert property.GetSetMethod() is null
assert property.GetGetMethod() is not null
assert property.GetGetMethod().IsAbstract
assert property.GetGetMethod().IsPublic
assert property.GetGetMethod().ReturnType is string
