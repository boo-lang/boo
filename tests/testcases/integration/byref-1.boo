import BooCompiler.Tests
import NUnit.Framework

value = 1
for i in -1, 0, 5:
	ByRef.SetValue(i, value)
	Assert.AreEqual(i, value)
	
	
ref = null
for o in object(), "", object():
	ByRef.SetRef(o, ref)
	Assert.AreSame(o, ref)
