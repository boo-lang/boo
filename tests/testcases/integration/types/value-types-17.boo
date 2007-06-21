"""
1 2 3
1
2
3
"""
struct Point:
	X as int
	Y as int
	
	def constructor(x, y):
		self.X = x
		self.Y = y
	
struct Point3D:	
	XY as Point
	Z as int
	
	def constructor(x, y, z):
		print x, y, z
		self.XY = Point(x, y)
		self.Z = z

p = Point3D(1, 2, 3)
print p.XY.X
print p.XY.Y
print p.Z
