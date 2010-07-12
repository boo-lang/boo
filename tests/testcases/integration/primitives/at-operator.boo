"""
foo
bar
"""
def foo():
	print "foo"
	return "foo"
	
def bar():
	print "bar"
	return "bar"
	
c = @(foo(), bar())
assert c == "bar"
