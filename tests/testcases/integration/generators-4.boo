"""
0 4 8 12 16
0 4 8 12 16
"""
import NUnit.Framework

generator = i*2 for i in range(10) if 0 == i % 2

e1 = generator.GetEnumerator()
e2 = generator.GetEnumerator()
Assert.IsFalse(e1 is e2, "IEnumerable instances must be distintic!")

for i in 0, 4, 8, 12, 16:
	Assert.IsTrue(e1.MoveNext())
	Assert.IsTrue(e2.MoveNext())
	Assert.AreEqual(i, e1.Current)
	Assert.AreEqual(i, e2.Current)

print(join(generator))
print(join(generator))

