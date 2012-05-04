"""
invalid-matrix-index.boo(10,3): BCE0022: Cannot convert 'S' to 'int'.
invalid-matrix-index.boo(10,6): BCE0022: Cannot convert 'S' to 'int'.
"""
struct S:
	i as int
	
m = matrix of int(2, 2)
s = S(i: 42)
m[s, s] = 0

