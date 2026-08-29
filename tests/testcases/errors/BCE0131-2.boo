"""
BCE0131-2.boo(4,22): BCE0131: Invalid combination of modifiers on 'Test': abstract, final.
"""
abstract final class Test:
  def Blah():
    print "Blah"
  static def Foo():
    print "Foo"

Test.Foo()
