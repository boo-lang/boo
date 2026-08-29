"""
BCE0143-1.boo(8,9): BCE0143: Cannot return from an ensure block.
"""
def foo():
	try:
		print "trying..."
	ensure:
		return

foo()
