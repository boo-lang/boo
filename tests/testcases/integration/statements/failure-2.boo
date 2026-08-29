"""
Testing failure with uncaught exception
failed
ensured
uncaught
"""

try:
	print "Testing failure with uncaught exception"
	try:
		raise System.Exception("WOW")
	except as System.ArithmeticException:
		print "caught"
	failure:
		print "failed"
	ensure:
		print "ensured"
except:
	print "uncaught"
