"""
0
1
2 3
9
90
900
false
caught it
yessir
1 2
3 4
5 6 7
8 9 10 11
12
"""

def doit():
	if true: return 12
	return 0
	
def doit2():
	if true: return


if true: print(0)
if true: print 1
if true: print 2, 3

//test multiple method invocations on one line:
if false: print(6);print(7);print(8); //none should be printed
print(9);  //should be printed

//test multiple macros on one line:
if false: print 60; print 70; print 80 //none should be printed
print 90  //should be printed

//test mix:
if false: print 600; print(700); print 800; //none should be printed
print(900)  //should be printed

if false: print "true"
else: print "false"

try: raise "uh oh"
except e: print "caught it"; print "yessir"

//test pass statement
if true: pass
if false: pass

//if true: pass; print "ok" //not allowed

if false: print "error"
else: pass

try: raise "error"
except e: pass

//if true: if true: //not allowed
//	print "ok"

if true: v = 1; v2=2;
print v, v2

if true: i as int = 3; x as string = "4"
print i, x

if true: a, b, c = 5,6,7
print a, b, c

if true: q,r=8,9;s=10;t as int=11;
print q,r,s,t

print doit()
doit2()
