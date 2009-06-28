"""
2
1
2
1
2
42
"""
#NB: for brevity we assume this test runs on a little-endian architecture (x86...)

struct Test:
	x as int
	y as int

def Arg(t as Test):
	unsafe tx as int = t.x, ty as int = t.y:
		print (*tx)
		print (*ty)

def ArgRef(ref t as Test):
	unsafe tx as int = t.x, ty as int = t.y:
		print (*tx)
		print (*ty)

def ArrayArg(data as (byte)):
	unsafe bp as byte = data:
		print (*bp)


t = Test()
unsafe tp as long = t.x, btp as byte = t.y:
	*tp = 0x0000000200000001
	print (*btp)
//tp
assert t.x == 1
assert t.y == 2
Arg(t)
ArgRef(t)

bytes = array[of byte](2)
bytes[0] = 42
ArrayArg(bytes)

