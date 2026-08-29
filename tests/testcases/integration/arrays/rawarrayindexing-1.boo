"""
normalarrayindexing is working
rawarrayindexing is working
"""

myarray = (1,2,3)

try:
	normalArrayIndexing:
		myarray[-1] = 4
	assert myarray[2] == 4
	print "normalarrayindexing is working"
except e:
	print "error: you should not see me - normalarrayindexing is not working"

try:
	rawArrayIndexing:
		myarray[-1] = 5
	print "error: you should not see me - rawarrayindexing macro is not working"
except e as System.IndexOutOfRangeException:
	print "rawarrayindexing is working"
