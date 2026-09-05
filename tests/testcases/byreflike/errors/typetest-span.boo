"""
typetest-span.boo(7,24): BCE0190: Cannot box byref-like type 'System.ReadOnlySpan[of char]': it has no conversion to 'object'.
"""
import System

span = "Hello".AsSpan()
Console.WriteLine(span isa object)
