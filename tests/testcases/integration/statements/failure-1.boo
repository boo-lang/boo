"""
Testing failure with caught exception
caught
ensured
"""

def RetVal():
	return 5
 
try:
	print "Testing failure with caught exception"
	try:
		raise System.Exception()
	except if RetVal() == 5:
		print "caught"
	failure:
		print "failed"
	ensure:
		print "ensured"
except:
	print "uncaught"
