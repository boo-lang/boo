import NUnit.Framework
import Generators from BooModules

current = 1
for i in oddNumbers(10):
	Assert.AreEqual(1, i % 2, "integer modulus")
	Assert.AreEqual(current, i)
	current += 2
