"""
BCE0070-2.boo(4,1): BCE0070: Recursive and mutually recursive methods must declare their return types.
"""
def foo(value as int):
	return bar(value) if value > 5
	return 1
	
def bar(value as int) as int:
	b = foo(value)
	return 1

