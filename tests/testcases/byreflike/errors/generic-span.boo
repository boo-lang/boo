"""
generic-span.boo(7,9): BCE0194: Byref-like type 'System.ReadOnlySpan[of char]' cannot be used as a generic argument.
"""
import System
import System.Collections.Generic

l = List[of ReadOnlySpan[of char]]()
Console.WriteLine(l.Count)
