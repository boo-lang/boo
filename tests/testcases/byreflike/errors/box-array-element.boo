"""
box-array-element.boo(8,8): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

span = "Hello".AsSpan()
a = array(object, 1)
a[0] = span
Console.WriteLine(a[0])
