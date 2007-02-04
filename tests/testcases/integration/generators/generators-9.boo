import NUnit.Framework

a = (i*5 for i in range(3), i*2 for i in range(3))

# Assert.AreSame(typeof((AbstractGenerator)), a.GetType())
Assert.AreEqual("0, 5, 10", join(a[0], ", "))
Assert.AreEqual("0, 2, 4", join(a[1], ", "))
