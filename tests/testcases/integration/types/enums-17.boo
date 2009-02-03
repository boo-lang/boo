enum PublicEnum:
	None
	
internal enum InternalEnum:
	None

def assertEnum(type as System.Type):
	assert type.IsEnum
	assert type.IsSerializable
	assert type.IsSealed
	assert type.IsValueType
	
assertEnum PublicEnum
assert typeof(PublicEnum).IsPublic
assertEnum InternalEnum
assert not typeof(InternalEnum).IsPublic
