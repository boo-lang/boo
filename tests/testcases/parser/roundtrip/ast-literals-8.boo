"""
node1 as Node = ast:
	return 3

node2 as Node = ast:
	return 42

print 'it works'
"""
node1 as Node = ast:
	return 3

node2 as Node = ast { return 42 }

print 'it works'