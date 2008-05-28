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
assert na == 31
assert not (na != 31)
print "ouch! na == 32" if 32 == na
na--



na *= 2
assert na is not null
assert 60 == na
assert 59 != na
assert not (59 == na)

na *= nb
assert na == 720
assert not (na != 720)

na *= nn
assert na is null
assert not (na is not null)

na = 30
assert na is not null
assert not (na is null)
na += nn
assert na is null
assert not (na is not null)

na = 30
nd as int? = na + nb + c + 64
assert nd == 30 + 12 + 42 + 64
assert not (nd != 30 + 12 + 42 + 64)

nd = na + nb + nn + c + 64 #since nn is null the whole expression is null
assert nd is null
assert not (nd is not null)

