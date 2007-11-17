"""
BCE0150-1.boo(4,17): BCE0150: 'final' cannot be applied to interface definitions.
"""
final interface IFoo:
  def Test()

class Bar(IFoo):
  def Test():
    print "Test"

Bar().Test()
