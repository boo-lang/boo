"""
Foo()
Foo(value)
Foo()
Foo(value)
Foo()
Foo(value)
Foo()
Foo(value)
"""
[Extension]
def Foo[of T](context as object):
	print "Foo()"

[Extension]
def Foo[of T](context as object, value as object):
	print "Foo(value)"

o = object()
Foo[of int?](o)
Foo[of int?](o, null)
o.Foo[of int?]()
o.Foo[of int?](null)

p = 42
Foo[of int?](p)
Foo[of int?](p, null)
p.Foo[of int?]()
p.Foo[of int?](null)


