class Point(System.ValueType):
	public X as int
	public Y as int
	
p = Point(X: 200, Y: 250)
assert 200 == p.X
assert 250 == p.Y

p.X = 300
assert 300 == p.X
