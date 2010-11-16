enum PublicEnum:
	None
	
internal enum InternalEnum:
	None

def assertEnum(type as System.Type):
	assert type.IsEnum
	assert type.IsSerializable
	assert type.IsSealed
	assert type.IsValueType
	value__ = type.GetField("value__")
	assert value__ is not null
	assert value__.IsPublic
	assert int is value__.FieldType
	
assertEnum PublicEnum
assert typeof(PublicEnum).IsPublic
assertEnum InternalEnum
assert not typeof(InternalEnum).IsPublic
