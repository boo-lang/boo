"""
box-duck.boo(7,13): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'duck'.
"""
import System

span = "Hello".AsSpan()
d as duck = span
Console.WriteLine(d.Length)
