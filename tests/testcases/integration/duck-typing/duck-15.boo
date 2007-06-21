"""
foo
bar
"""
import BooCompiler.Tests

r = ReturnDucks()
d = r.GetDuck(true)
print d.Foo()

d = r.GetDuck(false)
print d.Bar()
