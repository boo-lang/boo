"""
BCE0152-1.boo(5,7): BCE0152: 'Test' is marked 'abstract final' and can only have a static constructor.
"""
abstract final class Test:
  def constructor():
    print "Blah"
  static def Foo():
    print "Foo"

Test()
