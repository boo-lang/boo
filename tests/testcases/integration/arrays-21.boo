import NUnit.Framework

a = array(i*2 for i in range(3))
Assert.AreSame(a.GetType(), System.Type.GetType("System.Int32[]"))

Assert.AreEqual(2, a[-1]-a[-2])
Assert.AreEqual(-2, a[-2]-a[-1])
