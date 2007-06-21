import BooCompiler.Tests
import NUnit.Framework

value = 1
for i in -1, 0, 5:
	ByRef.ReturnValue(i, value)
	Assert.AreEqual(i, value)
	
	
reference = null
for o in object(), "", object():
	ByRef.ReturnRef(o, reference)
	Assert.AreSame(o, reference)
