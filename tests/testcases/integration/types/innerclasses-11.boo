"""
3
"""
import BooCompiler.Tests.SupportingClasses.OutterClass

struct Foo:
	x as int

class Bar:
	[property(F)] _f = Foo(x: 0)

b = Bar()
b.F.x += InnerClass.X
print b.F.x
