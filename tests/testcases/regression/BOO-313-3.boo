struct Point:
    x as int
    y as int

struct Rectangle:
    [property(topLeft)]
    private _top as Point

class Figure:
    [property(rect)]
    _rect as Rectangle

f = Figure()
// t1 = f.rect; t2 = t1.topLeft; t2.x = 10; t1.topLeft = t2; f.rect = t1;
f.rect.topLeft.x = 10

assert 10 == f.rect.topLeft.x
