"""
1
3
5
2
4
6
"""
import NUnit.Framework

def odds(begin, end):
	return i for i in range(begin, end) if i % 2
	
def evens(begin, end):
	return i for i in range(begin, end) unless i % 2
	
for expected, actual in zip((1, 3, 5), odds(1, 6)):
	print(actual)
	Assert.AreEqual(expected, actual)
	
for expected, actual in zip((2, 4, 6), evens(1, 7)):
	print(actual)
	Assert.AreEqual(expected, actual)
	

