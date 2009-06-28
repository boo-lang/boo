"""
42
42
255
255
"""
#NB: for brevity we assume this test runs on a little-endian architecture (x86...)

b as byte = 255
bytes = array[of byte](16)
bytes[0] = 42

unsafe bp as byte = bytes, bp2 as byte = bytes, ip as int = bytes[4]:
	System.Console.WriteLine(*bp)
	System.Console.WriteLine(*bp2)
	raise "KO1" unless *bp == 42 and bytes[0] == *bp and *bp == *bp2

	*bp = 255
	System.Console.WriteLine(*bp)
	System.Console.WriteLine(*bp2)
	raise "KO2" unless b == *bp and *bp2 == *bp

	*ip = 0x010042FF
//bp
assert bytes[0] == 0xFF
assert bytes[1] == 0
//ip (starts at offset 4)
assert bytes[4] == 0xFF
assert bytes[5] == 0x42
assert bytes[6] == 0
assert bytes[7] == 0x01


longs = array[of long](16)
longs[0] = 0x4200000000000000
unsafe bp as byte = longs, bp2 as short = longs:
	*bp2 = 0x10
	(*bp2)++ #0x11
	*bp2 <<= 8 #0x1100
	*bp = 0xFF
	*bp -= 1 #0xFE
//lbp
assert longs[0] == 0x42000000000011FE


floats = array(single, 16)
floats[0] = 1.0f
unsafe fp as single = floats, fp2 as single = floats:
	raise "KO fp 1" unless *fp == *fp2 and *fp < 1.1f
	*fp += 0.1f
	raise "KO fp 2" unless *fp == *fp2 and *fp > 1.0f
//fp
assert floats[0] > 1.0f and floats[0] < 1.2f


src = array[of byte](8)
src[3] = 1
dst = array[of byte](8)
dst[6] = 1
unsafe psrc as uint = src, pdst as uint = dst:
	#replace an uint at a time from src to dst only if src is not zero
	for i in range(len(src) / sizeof(psrc)):
		*pdst = *psrc unless *psrc == 0
		psrc++
		pdst++
assert dst[3] == 1 #this has been copied from src
assert dst[6] == 1 #this has not been replaced since src[4:8] is 0

