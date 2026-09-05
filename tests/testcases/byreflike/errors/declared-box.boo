"""
declared-box.boo(11,25): BCE0190: Cannot box byref-like type 'Buffer': it has no conversion to 'object'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer:
	public Length as int

boxed as object = Buffer()
Console.WriteLine(boxed)
