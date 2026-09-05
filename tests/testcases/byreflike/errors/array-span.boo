"""
array-span.boo(7,8): BCE0193: Byref-like type 'System.ReadOnlySpan[of char]' cannot be the element type of an array.
"""
import System

span = "Hello".AsSpan()
arr = (span,)
Console.WriteLine(arr.Length)
