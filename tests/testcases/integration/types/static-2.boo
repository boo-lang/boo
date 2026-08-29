"""
Baz sees 0
Foo.Bar()
Baz sees 1
"""
static class Foo:
  i = 0

  def Bar():
    print "Foo.Bar()"

  def Baz():
    print "Baz sees ${i}"
    i += 1

Foo.Baz()
Foo.Bar()
Foo.Baz()