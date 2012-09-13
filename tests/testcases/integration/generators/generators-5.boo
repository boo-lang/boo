"""
3
7
11
"""


def sumPairs(pairs):
	return i+j for i as int, j as int in pairs
	
pairs = ((1, 2), (3, 4), (5, 6))
expected = 3, 7, 11

for e, a in zip(expected, sumPairs(pairs)):
	print(a)
	assert e == a

