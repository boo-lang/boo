"""
def foo():
	return [|
		return 3
	|]


def bar():
	return [|
		print 'Hello, world'
	|]
"""
def foo():
	return [|
		return 3
	|]
def bar():
	return [| print 'Hello, world' |]
