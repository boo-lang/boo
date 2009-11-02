"""
1
2
3
*
0
"""

def gen(*items):
	for i in items:
		yield i
	or:
		yield 0

for i in gen(1, 2, 3):
	print i
	
print "*"

for i in gen():
	print i
