"""
Disposable.constructor
Disposable.foo
Disposable.Dispose
"""
import BooCompiler.Tests from BooCompiler.Tests

using disposable=Disposable():
	disposable.foo()
