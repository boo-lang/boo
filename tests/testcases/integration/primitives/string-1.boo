"""
foo(char)
"""
def foo(ch as char):
	print "foo(char)"
	
def foo(o as object):
	print "foo(object)"
	
for ch in "f":
	foo(ch)

