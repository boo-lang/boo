import NUnit.Framework

a = 2
b = do ():
	c = 4
	r as callable = { return a*c }
	w as callable = { value | c = value }
	return r, w
	
reader, writer = b()
Assert.AreEqual(8, reader())

writer(3)
Assert.AreEqual(6, reader())

a = 5
Assert.AreEqual(15, reader())

writer(2)
Assert.AreEqual(10, reader())


