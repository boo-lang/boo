"""
box-inherited-member.boo(12,29): BCE0190: Cannot box byref-like type 'Buffer': it has no conversion to 'System.ValueType'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer:
	public Length as int

b = Buffer()
Console.WriteLine(b.ToString())
