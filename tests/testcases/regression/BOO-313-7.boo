struct Point:
    x as int
    y as int

struct Rectangle:
    topLeft as Point

class Figure:
	[property(rect)]
	_rect as Rectangle

f = Figure()
f.rect.topLeft.x = 10

assert 10 == f.rect.topLeft.x
