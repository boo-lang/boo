"""
array-local.boo(6,1): BCE0193: Byref-like type 'System.Span[of int]' cannot be the element type of an array.
"""
import System

a as (Span[of int]) = null
Console.WriteLine(a)
