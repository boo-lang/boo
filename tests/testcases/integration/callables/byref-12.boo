"""
test1-ref int
10
test1-ref double
2.3
test2-int
3
test2-ref int
12
test3
21
test3
21
"""
import System.Globalization

//test overload resolution
def test1(ref x as int):
	print "test1-ref int"
	x = 10

def test1(ref x as double):
	print "test1-ref double"
	x = 2.3


//test overload resolution with multiple parameters
def test2(y as int, x as int):
	print "test2-int"
	x = 11

def test2(y as double, ref x as int):
	print "test2-ref int"
	x = 12

//test resolution when calling with non-exact params
def test3(y as double, ref x as int):
	print "test3"
	x = 21

a1 = 2
test1(a1)
print a1

a2 = 1.1
test1(a2)
print a2.ToString(CultureInfo.InvariantCulture)


ba1 = 2
bb = 3
test2(ba1,bb)
print bb

ba2 = 2.1
bb = 4
test2(ba2,bb)
print bb


ca1 = 2
cb = 3
test3(ca1,cb) //ca1 int converted to double
print cb

ca2 = 2.1
cb = 4
test3(ca2,cb)
print cb


