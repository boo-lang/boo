"""
w x y z
"""
class Foo:
    protected _x as string
    internal _y as string
    public _z as string

class Bar(Foo):
    private _w as string

    def constructor():
        _w = "w"
        _x = "x"
        _y = "y"
        _z = "z"

    def run():
        print _w, _x, _y, _z

Bar().run()
