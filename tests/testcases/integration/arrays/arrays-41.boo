"""
42
0
0
System.Int32
4
"""

a = array[of int](3)
a[0]=42
for i in a:
	print i

ints as int* = (of int: 1, 2, 3, 4,)
a = array(ints) #array[of int](int*) is inferred here
print a.GetType().GetElementType().FullName
print a.Length

