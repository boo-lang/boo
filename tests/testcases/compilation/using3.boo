"""
Disposable.constructor
before block
inside block
Disposable.Dispose
after block
"""
import BooCompiler.Tests from BooCompiler.Tests
import NUnit.Framework

class Foo:

	[property(Bar)] _d = Disposable()

f = Foo()
print("before block")
using f.Bar:
	print("inside block")
print("after block")
Assert.IsNull(f.Bar)
