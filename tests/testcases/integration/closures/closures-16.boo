
o1 = object()
o2 = object()

c1 = { return o1 }
c2 = { o1 = o2 }

assert o1 is c1()

c2()
assert o2 is c1(), "local variables must be shared between closures"
