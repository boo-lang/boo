import System.Runtime.InteropServices


[StructLayout(LayoutKind.Sequential, Size: 12, Pack: 4)]
struct Vector3d:
	x as single
	y as single
	z as single


b as byte = 9
assert sizeof(byte) == 1
assert sizeof(b) == 1
unsafe bp as byte = b:
	assert sizeof(*bp) == 1

s as short
assert sizeof(s) == 2

us as ushort
assert sizeof(us) == 2

i as int
assert sizeof(i) == 4
unsafe ip as int = i:
	assert sizeof(*ip) == 4

ui as uint
assert sizeof(ui) == 4

l as long
assert sizeof(l) == 8

ul as ulong
assert sizeof(ul)+1 == 8+1

f as single
assert sizeof(f) == 4

d as double
assert sizeof(d) == 8

v as Vector3d
assert sizeof(v) == 12

