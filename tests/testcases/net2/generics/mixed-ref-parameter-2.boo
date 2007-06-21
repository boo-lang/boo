import System.Collections.Generic



class Foo:

	pass



d = Dictionary[of Foo, string]()

f1 = Foo()


d[f1] = "Value"

s as string
d.TryGetValue(f1, s)


assert s == "Value"
