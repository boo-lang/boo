import BooCompiler.Tests
import NUnit.Framework

class Foo:
	public static value = 0
	public static reference

for i in -1, 0, 5:
	ByRef.SetValue(i, Foo.value)
	Assert.AreEqual(i, Foo.value)
	

for o in object(), "", object():
	ByRef.SetRef(o, Foo.reference)
	Assert.AreSame(o, Foo.reference)
