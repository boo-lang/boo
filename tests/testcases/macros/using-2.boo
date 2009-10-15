"""
before block
Disposable.constructor
inside block
Disposable.Dispose
after block
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

print("before block")
using Disposable():
	print("inside block")
print("after block")
