"""
a nullable arithmetic with one operand without value cannot be cast to a non-nullable
"""

na as int? = 30
nb as int? = 12
nn as int? #no value
c = 42

assert 42 == na+12
assert c == na+nb
#assert null == na+nn

try:
	c = na+nn
except InvalidOperationException:
	print "a nullable arithmetic with one operand without value cannot be cast to a non-nullable"

na++
na--

na *= 2
assert 60 == na

na *= nb
assert na == 720

na *= nn
assert na is null

na = 30
na += nn
assert na is null

na = 30
nd as int? = na + nb + c + 64
assert nd == 30 + 12 + 42 + 64

nd = na + nb + nn + c + 64 #since nn is null the whole expression is null
assert nd is null

