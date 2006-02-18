"""
Vector
"""

struct Vector:
	final X as int
	final Y as int
	def constructor(x as int, y as int):
		X = x
		Y = y

s = Vector(1,2)
print s


