"""
before
1
insideA
2
3
===
1
insideB
3
after
"""
def foo():
	try:
		print "1"
		yield
		print "2"
	ensure:
		print "3"
	
print "before"

for item in foo():
	print "insideA"
	assert item is null

print "==="

for item in foo():
	print "insideB"
	assert item is null
	break

print "after"
