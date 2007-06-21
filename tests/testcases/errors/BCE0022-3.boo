"""
BCE0022-3.boo(5,12): BCE0022: Cannot convert 'int' to 'string'.
"""
def foo() as string:
	return 3
	
def bar() as object:
	return "foo" if true
	return 5

print(foo())
