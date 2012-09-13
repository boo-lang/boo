"""
1
2
3
"""


def onetwothree():
	yield 1
	yield 2
	yield 3
	
for expected, actual in zip(range(1, 4), onetwothree()):
	print(actual)
	assert expected == actual
