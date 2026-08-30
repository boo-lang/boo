"""
high bit set: True
low bit set: True
zero: False
not high: False
while ran: True
"""

# A long is truthy when any of its 64 bits is set. Testing it as a 32 bit value
# would call every long whose low word is zero false.
def Truthy(value as long) as bool:
	if value:
		return true
	return false

high = 1L << 45
print "high bit set: ${Truthy(high)}"
print "low bit set: ${Truthy(1L)}"
print "zero: ${Truthy(0L)}"
print "not high: ${not high}"

ran = false
n = high
while n:
	ran = true
	n = 0L
print "while ran: ${ran}"
