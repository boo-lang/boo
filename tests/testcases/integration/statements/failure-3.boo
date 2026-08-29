"""
Testing failure with no exception
ensured
"""

try:
	print "Testing failure with no exception"
	try:
		pass
	except:
		print "caught"
	failure:
		print "failed"
	ensure:
		print "ensured"
except:
	print "uncaught"
