"""
Disposable.constructor
Boo.Lang.Compiler.Tests.Disposable
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

d as System.IDisposable = Disposable()
print(d.GetType())
