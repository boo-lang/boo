"""
before
evaluated
after
before
evaluated
evaluated
after
"""
import NUnit.Framework

def fun(value):
	print('evaluated')
	return value
	
a = null and true
Assert.IsNull(a)

b = true and 3
Assert.AreSame(int, b.GetType())
Assert.AreEqual(3, b)

print("before")
c = fun(false) and fun(true)
print("after")
Assert.IsFalse(c)

print("before")
d = fun(true) and fun(null)
print("after")
Assert.IsNull(d)

e = 0 and false
Assert.AreEqual(0, e)
