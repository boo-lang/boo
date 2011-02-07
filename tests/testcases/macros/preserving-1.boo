"""
foo: 42
bar[0]: False
"""
foo = 0
bar = [true]
preserving foo, bar[0]:
    foo = 42
    bar[0] = false
    print "foo: $foo"
    print "bar[0]: $(bar[0])"
assert foo == 0
assert bar[0] == true
