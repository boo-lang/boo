"""
generic-declared-box.boo(11,33): BCE0190: Cannot box byref-like type 'Buffer[of int]': it has no conversion to 'object'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer[of T]:
	public Length as int

boxed as object = Buffer[of int]()
Console.WriteLine(boxed)
