"""
foo
"""

breaker = "foo"

t = ("boo", "bar", "baz", "foo")

for item in t:
	found = item
	if item.Equals(breaker):
		break
else:
	print "We shouldn't be here!"
	
print found