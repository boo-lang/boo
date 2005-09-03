"""
TestNonByrefCallable with notbyref method True True
TestByrefCallable with byref method True True
callable.invoke True True
"""

callable TestNonByrefCallable(x as int, y as int) as int

callable TestByrefCallable(ref x as int, y as int) as int

def test_notbyref(x as int, y as int):
	x = 10
	return x+y
	
def test_byref(ref x as int, y as int):
	x = 11
	return x+y
	
c as TestNonByrefCallable = test_notbyref
x = 3
y = 4
print "TestNonByrefCallable with notbyref method", c(x,y)==14, x==3

x = 3
y = 4
cref as TestByrefCallable = test_byref
print "TestByrefCallable with byref method", cref(x,y)==15, x==11


x = 3
y = 4
print "callable.invoke", cref.Invoke(x,y)==15, x==11

