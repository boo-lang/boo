import NUnit.Framework
import BooCompiler.Tests from BooCompiler.Tests

Assert.AreEqual(28, Person(Age: 28).Age)
Assert.AreEqual(30, Person(Age: 30L).Age)
