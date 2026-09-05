"""
declared-field.boo(12,12): BCE0191: Byref-like type 'Buffer' cannot be a field of 'Holder'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer:
	public Length as int

class Holder:
	public b as Buffer

Console.WriteLine(Holder())
