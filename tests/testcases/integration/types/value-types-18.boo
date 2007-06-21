"""
33
45
"""
struct Point:
	X as int
	Y as int
	
	def constructor(x, y):
		self.X = x
		self.Y = y
		
class PointStore:
	
	public static Point as Point
	
PointStore.Point = Point(33, 45)
print PointStore.Point.X
print PointStore.Point.Y
