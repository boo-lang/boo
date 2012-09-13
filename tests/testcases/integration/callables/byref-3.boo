import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


class Foo:
	public static value = 0
	public static reference = null

for i in -1, 0, 5:
	ByRef.SetValue(i, Foo.value)
	assert i == Foo.value
	

for o in object(), "", object():
	ByRef.SetRef(o, Foo.reference)
	assert o is Foo.reference
