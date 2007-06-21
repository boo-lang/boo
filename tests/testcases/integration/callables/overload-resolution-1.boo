"""
object foo
int 42
"""

def foo(c as callable(int) as object):
	print "int", c(21)

def foo(c as callable(object) as object):
	print "object", c("foo")

foo({ o | return o })
foo({ i as int | return i*2 })
