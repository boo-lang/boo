"""
Disposable.constructor
before block
inside block
Disposable.Dispose
after block
"""
import BooCompiler.Tests from BooCompiler.Tests
import NUnit.Framework

d = Disposable()
print("before block")
using d:
	print("inside block")
print("after block")
Assert.IsNotNull(d)
