class Foo:
	
	enum LogLevel:
		None
		Info
		Error	

type = Foo.LogLevel

assert type.IsEnum
assert type.IsSerializable
assert type.IsNestedPublic

nested = typeof(Foo).GetNestedTypes()
assert 1 == len(nested)
assert type is nested[0]

		
