"""
Disposable.constructor
Disposable.Dispose
"""
import Boo.Tests.Lang.Compiler from Boo.Tests

class Derived(Disposable):
	pass

def dispose(disposable as System.IDisposable):
	disposable.Dispose()

dispose(Derived())
