"""
Disposable.constructor
Disposable.foo
Disposable.Dispose
"""
import Boo.Tests.Lang.Compiler from Boo.Tests

using disposable=Disposable():
	disposable.foo()
