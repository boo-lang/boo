"""
BCE0153-1.boo(4,17): BCE0153: 'final' can not be applied to interface definitions.
"""
final interface IFoo:
  def Test()

class Bar(IFoo):
  def Test():
    print "Test"

Bar().Test()
