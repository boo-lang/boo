"""
BCE0070-2.boo(9,9): BCE0070: Definition of 'BCE0070-2Module.bar' depends on 'BCE0070-2Module.foo' whose type could not be resolved because of a cycle. Explicitly declare the type of either one to break the cycle.
"""
def foo(value as int):
	return bar(value) if value > 5
	return 1
	
def bar(value as int):
	b = foo(value)
	return 1

