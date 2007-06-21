"""
0
1
2
0
1
0
2
"""
for i in range(3):
	print i
	
for i in range(3):
	break if i > 1
	print i
	
for i in range(3):
	continue if i == 1
	print i

