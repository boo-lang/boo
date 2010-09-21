class Foo:
	
	enum LogLevel:
		None
		Info
		Error	

type = Foo.LogLevel

assert type.IsEnum
assert type.IsSerializable
assert type.IsNestedPublic
assert type.IsSealed
assert type.IsValueType

value__ = type.GetField("value__")
assert value__ is not null
assert value__.IsPublic
assert int is value__.FieldType

nested = typeof(Foo).GetNestedTypes()
assert 1 == len(nested)
assert type is nested[0]

		
