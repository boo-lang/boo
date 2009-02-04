#ignore BO0-759 - generic generator methods are not supported
"""
1
2
3
"""
def reversed[of T](values as (T)) as T*:
	for i in range(len(values) - 1, -1):
		yield values[i]
		
for i in reversed((3, 2, 1)):
	print i
