"""
1
2
"""
struct Point:
	X as int
	Y as int
	
	def constructor(x, y):
		self.X = x
		self.Y = y

p = Point(1, 2)
print p.X
print p.Y
