"""
Buffer(3)
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Buffer:
	public Length as int
	override def ToString() as string:
		return "Buffer(${Length})"

b = Buffer()
b.Length = 3
Console.WriteLine(b.ToString())
