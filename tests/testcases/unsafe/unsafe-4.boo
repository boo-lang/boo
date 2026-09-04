"""
1
4
42
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


v = Vector()
unsafe vp as Vector = v.x:
	(*vp).x = 42
	(*vp).y = 2
	(*vp).z = 3
	print ((*vp).x)

assert v.x == 42
assert v.y == 2
assert v.z == 3


v = Vector()
unsafe vp as Vector = v: #direct addressing (no v.firstfield)
	(*vp).x = 1
	(*vp).y = 2
	(*vp).z = 3

assert v.x == 1
assert v.y == 2
assert v.z == 3


vv = array[of Vector](3)
unsafe vp as Vector = vv:
	(*vp).x = 1
	vp++
	(*vp).y = 2
	vp++
	(*vp).z = 3

assert vv[0].x == 1
assert vv[0].y == 0
assert vv[0].z == 0
assert vv[1].x == 0
assert vv[1].y == 2
assert vv[1].z == 0
assert vv[2].x == 0
assert vv[2].y == 0
assert vv[2].z == 3

