"""
1 2 3 1
"""
def foo():
	yieldAll range(1, 4)
	yield 1
	
print join(foo())
	

