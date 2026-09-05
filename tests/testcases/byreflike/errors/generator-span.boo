"""
generator-span.boo(6,5): BCE0192: Byref-like type 'System.ReadOnlySpan[of char]' cannot be captured by a closure, a generator or an async method.
"""
import System

def gen():
	span = "Hello".AsSpan()
	yield span.Length

for x in gen():
	Console.WriteLine(x)
