"""
foo
"""
def p(message):
	print message
	
def foo():
	return 42
	
p "foo" if foo() >= 42
p "bar" if foo() < 42
