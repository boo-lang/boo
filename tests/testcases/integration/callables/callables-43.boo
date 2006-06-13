import NUnit.Framework
	
a = { item | return item.ToString() }, { item as string | return item.ToUpper() }

Assert.AreEqual("3", a[0](3))
Assert.AreEqual("FOO", a[-1]("foo"))
