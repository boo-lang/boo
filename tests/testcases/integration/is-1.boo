"""
first
not null
second
null
"""
def test(o):
	print "not null" if o is not null
	print "null" if o is null
	
print "first"
test(object())
print "second"
test(null)
