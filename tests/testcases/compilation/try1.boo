"""
caught!
should have passed here first!
should end up here!

"""
try:
	raise System.ApplicationException("caught!")
	print("should not get here!")
except x:
	print(x.Message)
ensure:
	print("should have passed here first!")
print("should end up here!");
