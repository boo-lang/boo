import NUnit.Framework

a as object = 3
b as byte = a
c as int = a
d as long = a
e as single = a
f as double = a
g as short = a
h as uint = a
i as ulong = a
j as ushort = a

Assert.AreEqual(3, a)
Assert.AreEqual(3, b)
Assert.AreEqual(3, c)
Assert.AreEqual(3, d)
Assert.AreEqual(3, e)
Assert.AreEqual(3, f)
Assert.AreEqual(3, g)
Assert.AreEqual(3, h)
Assert.AreEqual(3, i)
Assert.AreEqual(3, j)
