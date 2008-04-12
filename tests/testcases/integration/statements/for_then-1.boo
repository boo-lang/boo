"""
We made it!
foo
"""

breaker = "none"

t = ("boo", "bar", "baz", "foo")

for item in t:
	found = item
	if item.Equals(breaker):
		break
then:
	print "We made it!"
	
print found