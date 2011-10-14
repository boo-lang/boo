import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


value = 1
for i in -1, 0, 5:
	ByRef.SetValue(i, value)
	assert i == value
	
	
reference = null
for o in object(), "", object():
	ByRef.SetRef(o, reference)
	assert o is reference

//test passing an array element by reference to external method:
arr = (1,2,3)
ByRef.SetValue(10, arr[0])
assert 10 == arr[0]
ByRef.SetValue(11, arr[1])
assert 11 == arr[1]
