"""
using-byreflike.boo(13,9): BCE0190: Cannot box byref-like type 'Res': it has no conversion to 'System.IDisposable'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Res(IDisposable):
	public N as int
	def Dispose():
		Console.WriteLine("disposed")

using r = Res():
	Console.WriteLine("body")
