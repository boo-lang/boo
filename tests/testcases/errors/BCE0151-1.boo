"""
BCE0151-1.boo(4,18): BCE0151: 'static' cannot be applied to interface, struct, or enum definitions.
"""
static interface IFoo:
  def Test()

class Bar(IFoo):
  def Test():
    print "Test"

Bar().Test()
