"""
Disposable.constructor
Disposable.Dispose
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class Derived(Disposable):
	pass

def dispose(disposable as System.IDisposable):
	disposable.Dispose()

dispose(Derived())
