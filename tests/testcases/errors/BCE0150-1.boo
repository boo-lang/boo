"""
BCE0150-1.boo(5,7): BCE0150: 'Test' is marked 'abstract final' and cannot contain instance member 'Blah'; all members must be marked static.
"""
abstract final class Test:
  def Blah():
    print "Blah"
  static def Foo():
    print "Foo"

Test.Foo()
