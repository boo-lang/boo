"""
foo(int, int)
foo(int, *object)
foo(int, *object)
foo(int, int)
foo(int, int, object)
foo(int, *object)
foo(int, *int)
"""
def foo(a as int, b as int):
	print "foo(int, int)"
	
def foo(a as int, b as int, c as object):
	print "foo(int, int, object)"
	
def foo(a as int, *args as (int)):
	print "foo(int, *int)"
	
def foo(a as int, *args):
	print "foo(int, *object)"
	
o as object = 1

foo(1, 2)
foo(o, o)
foo(1, o)
foo(o, 1)
foo(1, 1, o)
foo(1, o, o)
foo(1, 1, 2, 2)
