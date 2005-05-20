"""
a = Foo.Bar
a |= Foo.Baz
a |= (Foo.Spam | Foo.Eggs)
b = Zeng.Bar
b &= Zeng.Baz
"""
a = Foo.Bar
a |= Foo.Baz
a |= Foo.Spam | Foo.Eggs
b = Zeng.Bar
b &= Zeng.Baz
