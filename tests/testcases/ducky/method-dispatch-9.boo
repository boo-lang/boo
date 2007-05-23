"""
1 Point(1, 1) Point(2, 2)
"""
struct Point:
	public X as int
	public Y as int
	override def ToString():
		return "Point(${X}, ${Y})"
		
class Foo:
	def bar(i as int, *points as (Point)):
		print i, join(points)
		
d as duck = Foo()
d.bar(1, Point(X: 1, Y: 1), Point(X: 2, Y: 2))

