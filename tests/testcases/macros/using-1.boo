"""
Disposable.constructor
Disposable.foo
Disposable.Dispose
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests


using disposable=Disposable():
	disposable.foo()
assert disposable is not null	
