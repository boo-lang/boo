"""
BCE0099-1.boo(8,9): BCE0099: yield cannot be used inside a try, except or ensure block.
BCE0099-1.boo(10,9): BCE0099: yield cannot be used inside a try, except or ensure block.
BCE0099-1.boo(12,9): BCE0099: yield cannot be used inside a try, except or ensure block.
"""
def foo():
	try:
		yield 1
	except:
		yield 2
	ensure:
		yield 3

