import NUnit.Framework

predicate = { item as int | return 0 == item % 2 }

Assert.IsTrue(predicate(2))
Assert.IsFalse(predicate(3))
Assert.IsTrue(predicate(4))

