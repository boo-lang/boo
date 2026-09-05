"""
buf
"""
import System
import System.Runtime.CompilerServices

interface INamed:
	def Name() as string

[IsByRefLike]
struct Buffer(INamed):
	public Length as int
	def Name() as string:
		return "buf"

b = Buffer()
Console.WriteLine(b.Name())
