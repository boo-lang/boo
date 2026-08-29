"""
(42, 13)
"""
struct Point:
	X as int
	Y as int
	
	def constructor(x, y):
		self.X = x
		self.Y = y
	
	override def ToString():
		return "(${X}, ${Y})"
		
print Point(42, 13)
