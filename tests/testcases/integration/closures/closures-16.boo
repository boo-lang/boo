import NUnit.Framework

o1 = object()
o2 = object()

c1 = { return o1 }
c2 = { o1 = o2 }

Assert.AreSame(o1, c1())

c2()
Assert.AreSame(o2, c1(), "local variables must be shared between closures")
