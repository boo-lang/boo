"""
nameresolution0.boo(9,11): BCE0005: Unknown identifier: 'local'.
"""
def foo():
	local = "foo"
	bar()
	
def bar():
	print(local)
