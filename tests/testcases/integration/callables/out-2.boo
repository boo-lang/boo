import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


class Foo:
	public value = 0
	public reference = null

f = Foo()
for i in -1, 0, 5:
	ByRef.ReturnValue(i, f.value)
	assert i == f.value
	

for o in object(), "", object():
	ByRef.ReturnRef(o, f.reference)
	assert o is f.reference
