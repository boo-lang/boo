"""
boo
0
1
2
3
4
Did we make it?
"""

t = ("boo", "bar", "baz", "foo")

for item in t:
	print item
	for i in range(5): print i
	then: break
then:
	print "We shouldn't be here!"
	
print "Did we make it?"