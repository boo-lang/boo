"""
docstring-written-twice.boo(7,5): BCE0044: A docstring can only be written once, either above the body or within it.
"""
# One docstring above the body, another within it.
def Foo():
"""outer"""
	"""inner"""
	pass
