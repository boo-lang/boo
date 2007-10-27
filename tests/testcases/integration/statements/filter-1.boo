"""
Good
"""
def RetVal():
	return 5
	
try:
	raise System.Exception()
except as System.Exception unless RetVal() >= 4:
	print "What?"
except as System.Exception if RetVal().ToString() == "5":
	print "Good"
except as System.Exception:
	print "NO!"
