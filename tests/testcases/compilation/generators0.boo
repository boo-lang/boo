"""
0, 2, 4, 6
"""
def x2(item as int):
	return item*2

def map(fn as ICallable, iterator):
	return fn(item) for item in iterator	
	
print(join(map(x2, range(4)), ", "))
