"""
field-span.boo(7,12): BCE0191: Byref-like type 'System.ReadOnlySpan[of char]' cannot be a field of 'Holder'.
"""
import System

class Holder:
	public s as ReadOnlySpan[of char]

Console.WriteLine(Holder().s.Length)
