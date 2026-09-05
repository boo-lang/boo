"""
array-return.boo(6,15): BCE0193: Byref-like type 'System.Span[of int]' cannot be the element type of an array.
"""
import System

def make() as (Span[of int]):
	return null

Console.WriteLine(make())
