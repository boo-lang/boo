"""
caught!
should end up here!

"""
try:
	raise ApplicationException("caught!")
	print("should not get here!")
catch x:
	print(x.Message)
print("should end up here!");
