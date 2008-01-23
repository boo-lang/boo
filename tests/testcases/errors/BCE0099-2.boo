"""
BCE0099-2.boo(8,9): BCE0099: yield cannot be used inside a try, except or ensure block.
"""
def foo():
	try:
		yield 1
	ensure:
		yield 2

// the first yield is valid, only the second is invalid
