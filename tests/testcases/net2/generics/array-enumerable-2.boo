import System.Collections.Generic

class Foo:
	public def constructor():
		pass

a as IEnumerable of Foo
a = (Foo(), Foo())

assert a isa IEnumerable of Foo
assert a isa (Foo)
