"""
5
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer:
	public Length as int

b = Buffer()
b.Length = 5
Console.WriteLine(b.Length)
