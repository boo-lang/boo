import NUnit.Framework
import BooCompiler.Tests from BooCompiler.Tests

i0 as int = Constants.UnsignedInt
i1 as int = Constants.UnsignedLong
l0 as long = Constants.UnsignedLong
l1 as long = Constants.UnsignedInt

Assert.AreEqual(Constants.UnsignedInt, i0)
Assert.AreEqual(Constants.UnsignedLong, i1)
Assert.AreEqual(Constants.UnsignedLong, l0)
Assert.AreEqual(Constants.UnsignedInt, l1)
