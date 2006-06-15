enum E:
	None
	Foo = 1
	Bar = 2
	Baz = 4

[Extension]
static def op_Implicit(e as System.Enum) as bool:
	return cast(System.IConvertible, e).ToInt32(null) != 0

flags = E.Foo | E.Bar

value as bool = flags & E.Foo
assert value

value = flags & E.Bar
assert value

value = flags & E.Baz
assert not value

