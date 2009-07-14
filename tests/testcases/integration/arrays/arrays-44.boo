import System

def AreNearEqual(a as single, b as double):
	return Math.Abs(b - a) <= 0.000001

i = (0, 1, 1, 2, 3, 4)
s = (of single: 0, 1.0, 1.1, 2.0, 3.5, 4.0f)
sf = (0.0f, 1.0f, 1.1f, 2.0f, 3.5f, 4.0f) #same content, inferred
d = (of double: 0.0, 1.0, 1.1, 2.0f, 3.5, 4)

assert len(s) == 6
assert len(s) == len(d)
for x in range(len(s)):
	assert i[x] == Math.Floor(s[x])
	assert i[x] == Math.Floor(d[x])
	assert s[x] == sf[x]
	assert AreNearEqual(s[x], d[x])

