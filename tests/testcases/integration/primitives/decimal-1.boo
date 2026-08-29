def toDecimal(d as decimal):
	return d

def toInt(i as int):
	return i

o as object = 1
d as object = decimal(9)
assert toDecimal(o) == 1
assert toDecimal(d) == 9
assert toDecimal(12345) == 12345
assert toDecimal(45678L) == 45678
assert toDecimal(45678L).GetType() == decimal

y as decimal = 1.2
i as int = y
assert 1 == i
assert toInt(y) == 1

z as double = 1.2
y = y*y-y/(y+y)*2
z = z*z-z/(z+z)*2
assert y == z

assert -y == ((-1)*y)
assert -y == -z

assert y
assert false == (not y)
y = 0
assert not y

_sbyte as sbyte = 1
y = _sbyte
++y
y += 1
y = y*2.5
assert y == 7.5

print ("not 0") if y - y
assert y - 1

assert y and y
assert y or y
assert y == y

zero as decimal = 0
assert decimal == (zero and d).GetType()
assert 0 == zero

assert y > 2 and y > 1
assert false == (y < 1)

print ("y > 10??") if y > 10
print ("y <= 0??") if y <= 0
if y <= 1.15:
	print "y <= 1.15??"

b as decimal = y
assert y + b == 2*y
assert 2*y == 15

a = (1,2,b)
assert a[0] == 1
assert a[1] == 2
assert a[2] == b
c as (decimal) = array(decimal,0)
assert a.GetType() == c.GetType()
