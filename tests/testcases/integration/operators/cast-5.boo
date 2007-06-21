import System
import NUnit.Framework

a as object = 1
b as object = 1L
c as object = 1.0
d as object = cast(short, 1)
e as object = "1"

assert cast(int, a) == cast(int, b)
assert cast(int, a) == cast(int, c)
assert cast(int, a) == cast(int, d)

try:
	f = cast(int, e)
	Assert.Fail("Cannot cast string to int!")
except x as InvalidCastException:
	pass
