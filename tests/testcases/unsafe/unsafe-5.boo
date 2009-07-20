"""
1
4
128
Vec2(4,256)
Vec2(2,128)
Vec2(3,64)
Vec2(4,32)
"""

struct Vec2:
	public x as int
	public y as int
	def constructor(a as int):
		x = a
		y = a * 2
	override def ToString():
		return "Vec2(${x},${y})"


ia = (1,2,3,4)
unsafe ip as int = ia:
	firsti = *ip
	for i in range(len(ia)-1):
		*ip = *(ip+1)
		ip++
	*ip = firsti

assert ia[0] == 2
assert ia[1] == 3
assert ia[2] == 4
assert ia[3] == 1


iv = (Vec2(1),Vec2(2),Vec2(3),Vec2(4))
unsafe vp as Vec2 = iv:
	print ( (*(vp)).x )
	print ( (*(vp+3)).x )

	_ = *vp
	*vp = *(vp+3)

	(*vp).y = 256
	(*(vp+1)).y = 128
	(*(vp+2)).y = 64
	(*(vp+3)).y = 32

	vp++
	vp++
	print ( (*(vp-1)).y )

print iv[0]
print iv[1]
print iv[2]
print iv[3]

