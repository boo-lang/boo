"""
Disposable.constructor
BooCompiler.Tests.Disposable
"""
import BooCompiler.Tests from BooCompiler.Tests

d as System.IDisposable = Disposable()
print(d.GetType())
