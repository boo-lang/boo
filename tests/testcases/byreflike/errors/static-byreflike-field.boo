"""
static-byreflike-field.boo(9,19): BCE0191: Byref-like type 'System.Span[of int]' cannot be a field of 'Wrapper'.
"""
import System
import System.Runtime.CompilerServices

[IsByRefLike]
struct Wrapper:
	public static s as Span[of int]

Console.WriteLine(Wrapper.s.Length)
