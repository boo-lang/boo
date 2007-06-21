"""
Disposable.constructor
Disposable.foo
Disposable.Dispose
"""
import BooCompiler.Tests from BooCompiler.Tests
import NUnit.Framework

using disposable=Disposable():
	disposable.foo()
Assert.IsNotNull(disposable)	
