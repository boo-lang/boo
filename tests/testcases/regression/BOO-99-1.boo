import System.Reflection

class Foo:
	static def constructor():
		pass

constructors = typeof(Foo).GetConstructors(BindingFlags.Public|BindingFlags.Instance)
assert 1 == len(constructors), "Instance constructor expected."

constructors = typeof(Foo).GetConstructors(BindingFlags.NonPublic|BindingFlags.Static)
assert 1 == len(constructors), "Static constructor expected."

