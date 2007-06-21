struct Point:
    x as int
    y as int

struct Rectangle:
	[property(topLeft)]
	private _topLeft as Point

class Figure:
	public rect as Rectangle

f = Figure()
f.rect.topLeft.x = 10

assert 10 == f.rect.topLeft.x
