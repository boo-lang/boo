"""
closure-span.boo(1,1): BCE0192: Byref-like type 'System.ReadOnlySpan[of char]' cannot be captured by a closure, a generator or an async method.
"""
import System

span = "Hello".AsSpan()
f = { Console.WriteLine(span.Length) }
f()
