import NUnit.Framework

def adder(amount as int):
	return { value as int | return amount+value }

a1 = adder(3)
a2 = adder(5)

Assert.AreEqual(6, a1(3))
Assert.AreEqual(5, a1(2))
Assert.AreEqual(6, a2(1))
Assert.AreEqual(8, a2(3))
Assert.AreEqual(a1(4), a2(2))
