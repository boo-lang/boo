"""
0
2
4
6
8
"""
def foo():
	return [i*2 for i in range(5)]
	
print(join(foo(), "\n"))

