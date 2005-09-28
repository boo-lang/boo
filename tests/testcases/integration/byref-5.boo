"""
storerefobj 3 True
loadrefobj 8 True
passonobj 3 True
passbackobj 11 11 True True
storerefdouble 4.3 True
storerefchar b True
storerefdecimal 10.5555555 True
storerefshort 67 True
storerefarray System.Int32[] True
passonarray System.Int32[] True
passonarray2 System.Int32[] True
sliceref [2, 3] True
setsliceref [10, 2, 3] True
byrefarrayelement 10 2 3 True
byrefducky string True
"""

class MyClass:
	[Property(Property1)]
	_fld as int

def storerefobj(ref c as MyClass):
	c = MyClass(Property1 : 3)

def loadrefobj(ref c as MyClass):
	c.Property1 = 8 //don't even need ref param to do this
	
def passonobj(ref refparam as MyClass):
	storerefobj(refparam)

def passbackobj(ref c as MyClass):
	c.Property1 = 11
	return c

def storerefdouble(ref refparam as double):
	refparam = 4.3
def storerefchar(ref refparam as char):
	refparam = "b"[0]

def storerefdecimal(ref refparam as decimal):
	refparam = 10.5555555
def storerefshort(ref refparam as short):
	refparam = 67
def storerefarray(ref refparam as (int)):
	refparam = (4,5,6)
def passonarray(ref refparam as (int)):
	storerefarray(refparam)
def passonarray2(ref refparam as (int)):
	temp = refparam
	storerefarray(temp)
	refparam = temp
	
def sliceref(ref refparam as List):
	refparam = refparam[1:]
def setsliceref(ref refparam as List):
	refparam[0] = 10 //don't need ref param to do this:

def byrefarrayelement(ref x as int):
	x = 10
def byrefducky(ref d as duck):
	d = "string"
	

c1 = MyClass(Property1: 1)

storerefobj(c1)
print "storerefobj", c1.Property1, c1.Property1==3

c3 = MyClass(Property1: 10)

loadrefobj(c3)
print "loadrefobj", c3.Property1, c3.Property1==8

c3.Property1 = 10
passonobj(c3)
print "passonobj", c3.Property1, c3.Property1==3

c3.Property1 = 10
c4 = passbackobj(c3)
print "passbackobj", c3.Property1, c4.Property1, c3.Property1==11, c4.Property1==11

d as double = 1.5
c as char = "a"[0]
sh as short = 42
arr = (1,2,3)

storerefdouble(d)
print "storerefdouble", d.ToString(System.Globalization.CultureInfo.InvariantCulture), d==4.3
storerefchar(c)
print "storerefchar", c, c=="b"[0]

dec as decimal = 6.66666
storerefdecimal(dec)
print "storerefdecimal", dec.ToString(System.Globalization.CultureInfo.InvariantCulture), dec==10.5555555

storerefshort(sh)
print "storerefshort", sh, sh==67
storerefarray(arr)
print "storerefarray", arr, arr==(4,5,6)
arr = (1,2,3)
passonarray(arr)
print "passonarray", arr, arr==(4,5,6)
arr = (1,2,3)
passonarray2(arr)
print "passonarray2", arr, arr==(4,5,6)

l1 = [1,2,3]
sliceref(l1)
print "sliceref", l1, l1==[2,3]
l1 = [1,2,3]
setsliceref(l1)
print "setsliceref", l1, l1==[10,2,3]

arr = (1,2,3)
byrefarrayelement(arr[0])
print "byrefarrayelement", join(arr), arr==(10,2,3)

//passing a list element by reference will not work, however (test removed)

duckobj as duck = 3
byrefducky(duckobj)
print "byrefducky", duckobj, duckobj=="string"

