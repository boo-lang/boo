"""
1: success
2: success
4: success
6: success
"""
test as single = 0.0
if test == 0:
	print "1: success"	
if test != 1:
	print "2: success"	
if test:
	print "3: success"	
if not test:
	print "4: success"	

test2 as object = test
if test2:
	print "5: success"	
if not test2:
	print "6: success"
