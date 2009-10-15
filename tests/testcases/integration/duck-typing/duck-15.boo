"""
foo
bar
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

r = ReturnDucks()
d = r.GetDuck(true)
print d.Foo()

d = r.GetDuck(false)
print d.Bar()
