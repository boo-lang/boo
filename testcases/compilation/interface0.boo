"""
Disposable.constructor
Boo.Tests.Lang.Compiler.Disposable
"""
import Boo.Tests.Lang.Compiler from Boo.Tests

d as System.IDisposable = Disposable()
print(d.GetType())
