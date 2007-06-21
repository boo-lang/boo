"""
1, 2, 3
"""
[Extension]
def op_Implicit(a as System.Array) as string:
	return join(a, ', ')

a = (1, 2, 3)
s as string = a
print s

