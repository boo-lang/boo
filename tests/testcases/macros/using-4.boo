"""
Disposable.constructor
before block
inside block
Disposable.Dispose
after block
"""
import BooCompiler.Tests from BooCompiler.Tests

class Foo:

	[property(Bar)] _d = Disposable()

f = Foo()
print("before block")
using f.Bar:
	print("inside block")
print("after block")
assert f.Bar is not null
