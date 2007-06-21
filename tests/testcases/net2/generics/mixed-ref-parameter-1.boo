import System.Collections.Generic


class Foo:

	pass


d = Dictionary[of string, Foo]()

f1 = Foo()

d["Key"] = f1


f2 as Foo

d.TryGetValue("Key", f2)


assert f1 is f2
