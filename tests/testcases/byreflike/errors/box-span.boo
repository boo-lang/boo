"""
box-span.boo(6,33): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

boxed as object = "Hello".AsSpan()
Console.WriteLine(boxed)
