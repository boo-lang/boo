import NUnit.Framework

a = array(i*2 for i in range(3))
Assert.AreSame(a.GetType(), typeof((int)))

Assert.AreEqual(2, a[-1]-a[-2])
Assert.AreEqual(-2, a[-2]-a[-1])
