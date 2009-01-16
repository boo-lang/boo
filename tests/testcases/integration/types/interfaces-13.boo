interface IFoo:
	Bar:
		get
		
class Foo(IFoo):
	pass

type = Foo
assert 1 == len(type.GetInterfaces())
assert IFoo is type.GetInterfaces()[0]
assert not type.IsAbstract

bar = type.GetProperty("Bar")
assert bar is not null
assert bar.PropertyType is object
assert bar.GetGetMethod() is not null
assert bar.GetSetMethod() is null
assert bar.GetGetMethod().IsPublic
assert not bar.GetGetMethod().IsAbstract

