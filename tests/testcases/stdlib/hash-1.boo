import NUnit.Framework

h = Hash((i, i*2) for i in range(5))
Assert.AreEqual(5, len(h))
Assert.AreEqual([0, 1, 2, 3, 4], List(h.Keys).Sort())
Assert.AreEqual([0, 2, 4, 6, 8], List(h.Values).Sort())

for i in range(5):
	Assert.AreEqual(i*2, h[i])
