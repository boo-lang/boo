"""
Disposable.constructor
BooCompiler.Tests.SupportingClasses.Disposable
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

d as System.IDisposable = Disposable()
print(d.GetType())
