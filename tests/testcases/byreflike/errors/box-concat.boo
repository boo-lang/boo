"""
box-concat.boo(7,11): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

span = "Hello".AsSpan()
s = "x" + span
Console.WriteLine(s)
