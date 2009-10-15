"""
Disposable.constructor
Disposable.Dispose
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class Derived(Disposable):
	pass

def dispose(disposable as System.IDisposable):
	disposable.Dispose()

dispose(Derived())
