"""
Methods can be used as macros
some people think it might be confusing
"""
def withMessage(message, block as callable()):
	print message
	block()
	
withMessage "Methods can be used as macros":
	print "some people think it might be confusing"
	
