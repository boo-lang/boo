"""
before 1
before 2
in between
before 3
1
2
3
"""
def foo():
	print "before 1"
	yield 1
	print "before 2"
	yield 2
	print "before 3"
	yield 3
	
e = foo().GetEnumerator()

a, b = e
print "in between"
c, = e

print a
print b
print c
