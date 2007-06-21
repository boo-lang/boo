import BooCompiler.Tests

class Figure:
	[property(rect)]
	_rect as Rectangle

f = Figure()
f.rect.topLeft.x = 0
f.rect.topLeft.x += 10

assert 10 == f.rect.topLeft.x
