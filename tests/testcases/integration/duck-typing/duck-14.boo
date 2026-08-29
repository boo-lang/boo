"""
10
True
20
"""
class Point:
	public X as int
	public Y as int
	
class Sprite:
	[property(Position)]
	_position = Point()
	
s as duck = Sprite()
s.Position.X += 10
print s.Position.X
print s.Position.Y == 0

s.Position.X *= 2
print s.Position.X
