"""
BCE0151-2.boo(5,15): BCE0151: 'static' cannot be applied to interface, struct, or enum definitions.
BCE0151-2.boo(8,13): BCE0151: 'static' cannot be applied to interface, struct, or enum definitions.
"""
static struct Foo:
  test = 53

static enum FooEnum:
	Bar

print Foo.test
