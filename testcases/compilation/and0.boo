"""
True
True
3
evaluated
False
evaluated
evaluated
True
True
0
"""
def fun(value):
	print('evaluated')
	return value
	
a = null and true
print(a is null)

b = true and 3
print(b isa int)
print(b)

c = fun(false) and fun(true)
print(c)

d = fun(true) and fun(null)
print(d is null)

e = 0 and false
print(e isa int)
print(e)
