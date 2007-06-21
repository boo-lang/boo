"""
before first
1
before second
2
3
before third
4
"""

a = [1, 2, 3, 4]
e = a.GetEnumerator()

print "before first"
for item in e:
	print item
	break
	
print "before second"
for item in e:
	print item
	for another in e:
		print another
		break
	break
	
print "before third"
for item in e:
	print item
