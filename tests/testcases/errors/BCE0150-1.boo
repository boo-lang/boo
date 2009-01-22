"""
BCE0150-1.boo(6,17): BCE0150: 'final' cannot be applied to interface, struct, or enum definitions.
BCE0150-1.boo(13,14): BCE0150: 'final' cannot be applied to interface, struct, or enum definitions.
BCE0150-1.boo(16,12): BCE0150: 'final' cannot be applied to interface, struct, or enum definitions.
"""
final interface IFoo:
  def Test()

class Bar(IFoo):
  def Test():
    print "Test"

final struct FooStruct:
	public Foo as int

final enum FooEnum:
	Foo

Bar().Test()
