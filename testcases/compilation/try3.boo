"""
error!

"""
try:
	raise "error!"
except x as System.ApplicationException:
	print(x.Message)
