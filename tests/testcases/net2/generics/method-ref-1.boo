"""
42
42
"""
def identity[of T](value as T):
	return value
	
ioi = identity of int
ios = identity of string
print ioi(42)
print ios("42")
