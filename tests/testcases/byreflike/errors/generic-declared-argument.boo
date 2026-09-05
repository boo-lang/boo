"""
generic-declared-argument.boo(12,9): BCE0194: Byref-like type 'Buffer[of int]' cannot be used as a generic argument.
"""
import System
import System.Collections.Generic
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer[of T]:
	public Length as int

l = List[of Buffer[of int]]()
Console.WriteLine(l.Count)
