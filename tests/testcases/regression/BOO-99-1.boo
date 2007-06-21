import NUnit.Framework
import System.Reflection

class Foo:
	static def constructor():
		pass

constructors = typeof(Foo).GetConstructors(BindingFlags.Public|BindingFlags.Instance)
Assert.AreEqual(1, len(constructors), "Instance constructor expected.")

constructors = typeof(Foo).GetConstructors(BindingFlags.Public|BindingFlags.Static)
Assert.AreEqual(1, len(constructors), "Static constructor expected.")

