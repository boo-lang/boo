"""
Good
System.Exception: This is the message
"""

def RetVal(ref catch as bool):
	catch = not catch
	return not catch

catch = false
	
try:
	raise System.Exception("This is the message")
except ex as System.Exception if RetVal(catch):
	print "What?"
	print ex.GetType() + ": " + ex.Message
except ex as System.Exception unless RetVal(catch):
	print "Still Bad..."
	print ex.GetType() + ": " + ex.Message
except ex as System.Exception unless RetVal(catch):
	print "Good"
	print ex.GetType() + ": " + ex.Message
except ex as System.Exception:
	print "NO!"
	print ex.GetType() + ": " + ex.Message
