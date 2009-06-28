"""
"""
#NB: for brevity we assume this test runs on a little-endian architecture (x86...)

bytes = array[of byte](16)
unsafe p as uint = bytes: #fill 4 bytes at a time
	for i in range(len(bytes) / sizeof(p)):
		*p = uint.MaxValue
		p++

for b in bytes:
	assert b == byte.MaxValue


unsafe p as ulong = bytes: #fill 8 bytes at a time
	*p = 0
	p++
	*p = ulong.MaxValue
	p++
	p-- #oops we wanted 0 not max, go back and write again
	*p = 0

for b in bytes:
	assert b == 0


delta = 1
bdelta = 15
unsafe p as uint = bytes, bp as byte = bytes:
	*p = uint.MaxValue
	p = p+delta
	*p = uint.MaxValue
	p += delta
	*p = uint.MaxValue
	p += 1
	*p = 0x00FFFFFF
	bp += bdelta #no sizeof pointer normalization
	*bp = 0xFF

for b in bytes:
	assert b == byte.MaxValue

