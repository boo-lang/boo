import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


class Foo:
	public value = 0
	public reference = null

f = Foo()
for i in -1, 0, 5:
	ByRef.SetValue(i, f.value)
	assert i == f.value
	

for o in object(), "", object():
	ByRef.SetRef(o, f.reference)
	assert o is f.reference
