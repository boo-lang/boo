"""
before
foo
inside
bar
inside
baz
after
"""
def foo():
	print "foo"
	yield
	print "bar"
	yield
	print "baz"
	
print "before"

for item in foo():
	print "inside"
	assert item is null
	
print "after"
