"""
box-return.boo(7,26): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

def wrap() as object:
	return "Hello".AsSpan()

Console.WriteLine(wrap())
