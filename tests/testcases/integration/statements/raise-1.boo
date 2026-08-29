"""
error!

"""
try:
	raise "error!"
except x as System.ApplicationException:
	print "String is raised as ${x.GetType()}"
except x as System.Exception:
	print(x.Message)
