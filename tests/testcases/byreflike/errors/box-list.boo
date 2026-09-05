"""
box-list.boo(7,6): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

span = "Hello".AsSpan()
l = [span]
Console.WriteLine(l.Count)
