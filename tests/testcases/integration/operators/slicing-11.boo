import NUnit.Framework

a = "Gooooooooood niiiiiiiiight ding ding ding ding ding ding"
Assert.AreEqual("Go", a[:2])
Assert.AreEqual("Go", a[0:2])
Assert.AreEqual("ding", a[-4:])
Assert.AreEqual("din", a[-4:-1])


