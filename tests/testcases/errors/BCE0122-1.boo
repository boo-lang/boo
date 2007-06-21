"""
BCE0122-1.boo(6,12): BCE0122: Value type 'Foo' does not provide an implementation for 'System.IDisposable.Dispose()'. Value types cannot have abstract members.
"""
import System

struct Foo(IDisposable):
     pass
