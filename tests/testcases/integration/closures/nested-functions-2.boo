"""
42
42
"""
def makeCounter(start as int):
	def counter():
		return ++start
	return counter
	
counter = makeCounter(41)
print counter()
print counter() - 1
