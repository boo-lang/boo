"""
a = ast:
	return 3

d = ast:
	print 'Hello, world'

e = ast:
	print('Hello, world')
"""
a = ast { return 3 }
d = ast { print "hello world" }
e = ast { print("hello world") }
