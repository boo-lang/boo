class Point(System.ValueType):
	public X as int
	public Y as int
	
type = Point
assert type.IsPublic
assert type.IsValueType
assert type.IsSealed
