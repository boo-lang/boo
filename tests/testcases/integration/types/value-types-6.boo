"""
(200, 300)
"""
class Point(System.ValueType):
	public X as int
	public Y as int
	
	override def ToString():
		return "(${X}, ${Y})"
	
p = Point(X: 200, Y: 300)
print p

