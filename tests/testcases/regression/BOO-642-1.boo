"""
0
1
"""
struct Vector3:
	x as int
	y as int
	z as int

class Foo:
	def spam():
		yield 0	
		vec as Vector3
		vec.x = 1
		print vec.x

for item in Foo().spam():
	print item

