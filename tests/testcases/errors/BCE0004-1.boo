"""
BCE0004-1.boo(11,4): BCE0004: Ambiguous reference 'foo': BCE0004_1Module.foo(string), BCE0004_1Module.foo(int).
"""
def foo(i as int):
    print "int"

def foo(s as string):
    print "string"

o as object = "foo"
foo(o)
