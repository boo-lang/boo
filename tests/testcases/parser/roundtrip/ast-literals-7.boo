"""
class Foo:

	node1 = ast:
		return 3


	node2 = ast:
		return 42


print 'it works'
"""
class Foo:
	node1 = ast:
		return 3

	node2 = ast { return 42 }
	
print 'it works'