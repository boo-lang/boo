"""
Disposable.constructor
before block
inside block
Disposable.Dispose
after block
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


d = Disposable()
print("before block")
using d:
	print("inside block")
print("after block")
assert d is not null
