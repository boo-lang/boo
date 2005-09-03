"""
storeref 4 True
storeref2 12 True
storerefstring new True
passon 4 True
passon2 4 True
loadref 6 7 True True
loadref 2 True
passback 8 8 True True
passback2 2 2 True True
unpacking 9 10 True True
"""

def storeref(ref refparam as int):
	refparam = 4
	
def passon(ref refparam as int):
	storeref(refparam)

def passon2(ref refparam as int):
	temp = refparam
	storeref(temp)
	refparam = temp

def loadref(ref refparam as int):
	refparam = 6
	temp = refparam + 1
	return temp

def loadref2(ref refparam as int):
	temp = refparam
	++temp //just to avoid warning

def passback(ref refparam as int):
	refparam = 8
	return refparam

def passback2(ref refparam as int):
	return refparam

def unpacking(ref x as int, ref y as int):
	x, y = 9, 10

def storeref2(ref refparam as int):
	refparam = 4 + 4 + 4
	
def storerefstring(ref refparam as string):
	refparam = "new"
	
x = 2

storeref(x)
print "storeref", x, x==4
storeref2(x)
print "storeref2", x, x==12

s1 = "old"
storerefstring(s1)
print "storerefstring", s1, s1=="new"

x = 2
passon(x)
print "passon", x, x==4

x = 2
passon2(x)
print "passon2", x, x==4

x = 2
val = loadref(x)
print "loadref", x, val, x==6, val==7

x = 2
loadref2(x)
print "loadref", x, x==2

x=2
val = passback(x)
print "passback", x, val, x==8, val==8

x=2
val = passback2(x)
print "passback2", x, val, x==2, val==2

x = 2
y = 3
unpacking(x, y)
print "unpacking", x, y, x==9, y==10

