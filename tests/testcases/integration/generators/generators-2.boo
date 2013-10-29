"""
1
3
5
2
4
6
"""


def odds(begin, until):
	return i for i in range(begin, until) if i % 2
	
def evens(begin, until):
	return i for i in range(begin, until) unless i % 2
	
for expected, actual in zip((1, 3, 5), odds(1, 6)):
	print(actual)
	assert expected == actual
	
for expected, actual in zip((2, 4, 6), evens(1, 7)):
	print(actual)
	assert expected == actual
	

