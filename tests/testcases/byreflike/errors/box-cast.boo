"""
box-cast.boo(7,19): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

span = "Hello".AsSpan()
Console.WriteLine(cast(object, span))
