"""
array-builtin.boo(6,10): BCE0193: Byref-like type 'System.Span[of int]' cannot be the element type of an array.
"""
import System

a = array(Span[of int], 3)
Console.WriteLine(a.Length)
