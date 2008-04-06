"""
no value exception
okthxbye
"""

import System


class TestNullableFieldInitializer:
	def constructor():
		assert n == 1

	protected n as int? = 1


i = 69

a as int?
b as int? = i

assert a is null
assert b is not null
assert a == null
assert b != null
assert null == a
assert null != b
assert a != 42
assert 42 != a
assert b == 69
assert 69 == b

l = 0L
try:
	l = a
except InvalidOperationException:
	print "no value exception"

a = 42
assert a is not null
assert a == 42
assert a != 43
l = a
assert l = 42

a = null
assert a is null
assert a != 42

TestNullableFieldInitializer()

print "okthxbye"
