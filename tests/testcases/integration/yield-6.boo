import NUnit.Framework

def xrange(begin as int, end as int):
	assert end >= begin
	i = begin
	while i < end:
		yield i
		++i
		
Assert.AreEqual("0, 1, 2", join(xrange(0, 3), ", "))
Assert.AreEqual("5, 6, 7", join(xrange(5, 8), ", "))
