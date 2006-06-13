"""
true
inside foo
FOO
false
inside bar
BAR
"""
def eval(condition):
	return (foo() if condition else bar())
	
def foo():
	print "inside foo"
	return "foo"
	
def bar():
	print "inside bar"
	return "bar"
	
print "true"
print eval(true).ToUpper()
print "false"
print eval(false).ToUpper()

