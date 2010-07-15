"""
evaluated
if
evaluated
inside
"""
def fun():
	print 'evaluated'
	return 'inside'
	
a = fun() or fun()
print 'if'
if fun() or fun():
	print a
