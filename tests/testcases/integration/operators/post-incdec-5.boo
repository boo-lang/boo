"""
boo!
0
boo!
1
2
boo!
2
1
"""
def hasSideEffect(index):
	print "boo!"
	return index
	
i = (0,)
print i[hasSideEffect(0)]++
print i[hasSideEffect(0)]++
print i[0]
print i[hasSideEffect(0)]--
print i[0]
