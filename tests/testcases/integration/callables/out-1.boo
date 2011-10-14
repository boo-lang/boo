import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


value = 1
for i in -1, 0, 5:
	ByRef.ReturnValue(i, value)
	assert i == value
	
	
reference = null
for o in object(), "", object():
	ByRef.ReturnRef(o, reference)
	assert o is reference
