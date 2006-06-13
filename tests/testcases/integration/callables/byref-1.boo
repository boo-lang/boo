import BooCompiler.Tests
import NUnit.Framework

value = 1
for i in -1, 0, 5:
	ByRef.SetValue(i, value)
	Assert.AreEqual(i, value)
	
	
reference = null
for o in object(), "", object():
	ByRef.SetRef(o, reference)
	Assert.AreSame(o, reference)

//test passing an array element by reference to external method:
arr = (1,2,3)
ByRef.SetValue(10, arr[0])
Assert.AreEqual(10, arr[0])
ByRef.SetValue(11, arr[1])
Assert.AreEqual(11, arr[1])
