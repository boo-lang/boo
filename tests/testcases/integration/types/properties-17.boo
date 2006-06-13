import NUnit.Framework

interface IFoo:

	Foo as int:
		get
		set

	Bar:
		get

type = IFoo
foo = type.GetProperty("Foo")
assert foo is not null
assert int is foo.PropertyType
assert foo.GetGetMethod() is not null
assert foo.GetSetMethod() is not null
assert int is foo.GetGetMethod().ReturnType
assert void is foo.GetSetMethod().ReturnType

bar = type.GetProperty("Bar")
assert bar is not null
assert object is bar.PropertyType
assert bar.GetGetMethod() is not null
assert bar.GetSetMethod() is null

getter = bar.GetGetMethod()
assert 0 == len(getter.GetParameters())
assert object is getter.ReturnType
