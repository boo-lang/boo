"""
Good
System.Exception: This is the message
"""
def RetVal():
	return 5
	
try:
	raise System.Exception("This is the message")
except ex as System.Exception if RetVal() >= 4 and ex.Message != "This is the message":
	print "What?"
	print ex.GetType() + ": " + ex.Message
except ex as System.Exception unless RetVal().ToString() == "4":
	print "Good"
	print ex.GetType() + ": " + ex.Message
except ex as System.Exception:
	print "NO!"
	print ex.GetType() + ": " + ex.Message
