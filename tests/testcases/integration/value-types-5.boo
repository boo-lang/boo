class Point(System.ValueType):
	[property(X)]
	_x as int
	
	[property(Y)]
	_y as int
	
	def constructor(x, y):
		_x = x
		_y = y
	
p = Point(200, 250)
assert 200 == p.X
assert 250 == p.Y
