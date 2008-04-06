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

na *= 2
assert 60 == na

na *= nb
assert na == 720

#na *= nn
#assert na is null
