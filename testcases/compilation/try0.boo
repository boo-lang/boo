"""
caught!
should end up here!

"""
try:
	raise System.ApplicationException("caught!")
	print("should not get here!")
except x:
	print(x.Message)
print("should end up here!");
