"""
-3
32767
SHORT OVERFLOW
-32768
-4
ULONG OVERFLOW
"""

i = 3
print(-i)

d as double = -1.0
d = -d
assert d == 1.0

s as short = -32767 #-short.MaxValue
s = -s
print s
s = short.MinValue #-32768
try:
	s = -s
except:
	print "SHORT OVERFLOW"

unchecked:
	s = -s
print s

l as long = 4
l = -l
print l

u as ulong = 424242
try:
	u = -u
except:
	print "ULONG OVERFLOW"

