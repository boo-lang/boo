"""
got here first!
nested!
first!
second!

"""
using System

try:	
	raise ApplicationException("got here first!")
	print("never here!");
except x as ApplicationException:
	print(x.Message)
	try:
		raise ApplicationException("nested!");
	except x:
		print(x.Message)
	ensure:
		print("first!")
ensure:
	print("second!")
