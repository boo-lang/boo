"""
only once
0 0 1 0
"""
def foo():
	print "only once"
	return 2

a = array(int, 4)
a[foo()] += 1
print join(a)
