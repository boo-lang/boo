"""
foo
"""
class Foo:
    private _x as object

class Bar(Foo):
    private _x as string

    def constructor():
        _x = "foo"

    def run():
        print _x

Bar().run()
