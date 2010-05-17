class Size(System.ValueType):
	
	[property(Width)]
	_w as int
	
	[property(Height)]
	_h as int
	
s as Size
s.Width = 100
s.Height = 200
assert 100 == s.Width
assert 200 == s.Height
