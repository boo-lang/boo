"""
foo(*string)
foo(*object)
"""
def foo(*args):
	print "foo(*object)"
	
def foo(*args as (string)):
	print "foo(*string)"
	
foo("foo", "bar")
foo(("foo", "bar"))
