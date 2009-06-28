"""
1
4
"""

import System.Runtime.InteropServices


[StructLayout(LayoutKind.Sequential, Size: 24)]
struct Vector:
	x as int
	y as int
	z as int

	static sx as int
	static sy as int
	static sz as int

	def Flda():
		unsafe v as int = self.x:
			*v = 1
			v++
			*v = 2
			v++
			*v = 3


	def Sflda():
		unsafe v as int = self.sx:
			*v = 4
			v++
			*v = 5
			v++
			*v = 6


v = Vector()
v.Flda()
v.Sflda()

print v.x
print v.sx
assert v.x == 1
assert v.y == 2
assert v.z == 3
assert v.sx == 4
assert v.sy == 5
assert v.sz == 6

