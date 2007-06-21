class FinalFields:
	
	public static final Bar = object()
	
	public static final Foo = 1
	
	public static final Baz = "Baz"
	

type = FinalFields

field = type.GetField("Foo")
assert field.IsPublic
assert field.IsLiteral
assert field.IsStatic
assert int is field.FieldType
assert 1 == field.GetValue(null)
assert 1 == FinalFields.Foo
assert field.GetValue(null) isa int

field = type.GetField("Bar")
assert field.IsPublic
assert field.IsInitOnly
assert field.IsStatic
assert object is field.FieldType
assert field.GetValue(null) is not null

field = type.GetField("Baz")
assert field.IsPublic
assert field.IsLiteral
assert field.IsStatic
assert "Baz" == field.GetValue(null)
assert "Baz" == FinalFields.Baz

