import NUnit.Framework

a = (i*5 for i in range(3),)

Assert.AreSame(System.Type.GetType("System.Collections.IEnumerable[]"), a.GetType())
Assert.AreEqual("0, 5, 10", join(a[0], ", "))
