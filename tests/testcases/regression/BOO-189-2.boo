class Size(System.ValueType):
	
	[property(Width)]
	_w = 0
	
	[property(Height)]
	_h = 0
	
s as Size
s.Width = 100
s.Height = 200
assert 100 == s.Width
assert 200 == s.Height
