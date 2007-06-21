import NUnit.Framework

def x2(item as int):
	return item*2

def map(fn as ICallable, iterator):
	return fn(item) for item in iterator	
	
expected = "0, 2, 4, 6"
actual = join(map(x2, range(4)), ", ")

Assert.AreEqual(expected, actual)
