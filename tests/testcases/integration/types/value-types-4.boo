class Point(System.ValueType):
	public X as int
	public Y as int
	
	def constructor(x, y):
		self.X = x
		self.Y = y
	
p = Point(200, 250)
assert 200 == p.X
assert 250 == p.Y
