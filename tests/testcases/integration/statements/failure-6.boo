"""
Testing failure without ensure or catch
failed
uncaught
"""

try:
	print "Testing failure without ensure or catch"
	try:
		raise System.Exception()
	failure:
		print "failed"
except:
	print "uncaught"
