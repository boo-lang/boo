"""
Testing failure with uncaught exception, no ensure
failed
uncaught
"""

try:
	print "Testing failure with uncaught exception, no ensure"
	try:
		raise System.Exception("WOW")
	except as System.ArithmeticException:
		print "caught"
	failure:
		print "failed"
except:
	print "uncaught"
