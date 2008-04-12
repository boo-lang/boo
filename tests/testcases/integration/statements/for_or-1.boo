"""
foo
"""

breaker = "none"

t = ("boo", "bar", "baz", "foo")

for item in t:
	found = item
	if item.Equals(breaker):
		break
or:
	print "No items!"
	
print found