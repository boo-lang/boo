"""
caught!
should have passed here first!
should end up here!

"""
try:
	raise ApplicationException("caught!")
	print("should not get here!")
catch x:
	print(x.Message)
ensure:
	print("should have passed here first!")
print("should end up here!");
