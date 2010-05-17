import NUnit.Framework

l = [2, 4, 3, 5, 1]
Assert.AreEqual([1, 2, 3, 4, 5], l.Sort())
Assert.AreEqual([5, 4, 3, 2, 1],  l.Sort({ lhs as int, rhs as int | return rhs - lhs }))
