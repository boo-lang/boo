"""
8
"""

cache = {}

def fib(i as int) as int:
	r = cache[i]
	if r is null:
		if i < 2:
			r = 1
		else:
			r = fib(i-1) + fib(i-2)
	cache[i] = r
	return r
	
print fib(5)