"""
4
8
12
16
"""
def foo():
	return [i*2 for i in range(10) if i % 2]
	
print(join(foo(), "\n"))

