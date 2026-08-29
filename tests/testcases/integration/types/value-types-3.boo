class Point(System.ValueType):
	public X as int
	public Y as int
	
p1 = Point(X: 200, Y: 300)
assert 200 == p1.X
assert 300 == p1.Y

p2 = p1
p1.X = 250

assert 200 == p2.X
assert 300 == p2.Y
