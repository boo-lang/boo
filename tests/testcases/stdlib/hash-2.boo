import NUnit.Framework

h = { (1, 2): "1, 2", (3, 4): "3, 4" }
		
Assert.AreEqual("1, 2", h[(1, 2)])
Assert.AreEqual("3, 4", h[(3, 4)])

a = 3
b = 4
c = a, b
Assert.AreEqual("3, 4", h[c])

d = 1
e = 2
f = d, e
Assert.AreEqual("1, 2", h[f])
