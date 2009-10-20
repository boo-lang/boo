#ignore BOO-759 - generic generator methods are not supported
"""
2
4
6
"""
def reversed[of T](values as (T)) as T*:
	for i in range(len(values) - 1, -1):
		yield values[i]
		
for i in reversed((3, 2, 1)):
	print i * 2
