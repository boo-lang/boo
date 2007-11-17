"""
BCE0151-2.boo(4,15): BCE0151: 'static' cannot be applied to interface or struct definitions.
"""
static struct Foo:
  test = 53

print Foo.test
