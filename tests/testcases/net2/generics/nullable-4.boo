"""
na has value
nn has no value
na has value but nn hasn't
"""

na as int? = 42
nn as int? #no value

if na:
	print "na has value"
if not na:
	print "na has no value"

if nn:
	print "nn has value"
if not nn:
	print "nn has no value"

if na and not nn:
	print "na has value but nn hasn't"

assert na
assert not nn

