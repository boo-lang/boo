import NUnit.Framework

fname, lname = @/ /.Split("Eric Idle")
Assert.AreEqual("Eric", fname)
Assert.AreEqual("Idle", lname)

fname, lname = /\u0020/.Split("John Cleese")
Assert.AreEqual("John", fname)
Assert.AreEqual("Cleese", lname)

