"""
is int
'string' is a string
"""
o as object = 3
if (o is string):
	print("is string")
else:
	if (o is int):
		print("is int")
	else:
		print("what?")
if ("string" is not string):
	print("not a string")
else:
	print("'string' is a string")
