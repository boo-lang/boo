"""
Hello World
Hello World
"""
def workingArgs(*args as (string)):
	print join(args)
	
notWorkingArgs = def(*args as (string)):
	print join(args)
	
workingArgs("Hello", "World")
notWorkingArgs("Hello", "World")
