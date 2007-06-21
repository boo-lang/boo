"""
BCE0022-6.boo(10,5): BCE0022: Cannot convert 'Foo' to 'System.IDisposable'.
"""
import System

final class Foo:
	pass

f = Foo()
d = cast(IDisposable, f)
d.Dispose()
