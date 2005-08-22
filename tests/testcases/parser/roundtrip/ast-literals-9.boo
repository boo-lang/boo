"""
a = ast:
	print 'foo'

b = ast:
	System.Console.WriteLine('foo')
	System.Console.WriteLine('bar')
"""
a = ast:
	print 'foo'

// ast blocks must always parse expressions as statements	
b = ast:
	System.Console.WriteLine('foo')
	System.Console.WriteLine('bar')
