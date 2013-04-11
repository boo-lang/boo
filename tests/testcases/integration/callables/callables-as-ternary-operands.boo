"""
foo
bar
"""
def choose(condition):
	choice = (foo if condition else bar)
	choice()

def foo():
	print 'foo'

def bar():
	print 'bar'

choose true
choose false
