"""
is int
'string' is a string
"""
o as object = 3
if (o isa string):
	print("is string")
else:
	if (o isa int):
		print("is int")
	else:
		print("what?")
if (not "string" isa string):
	print("not a string")
else:
	print("'string' is a string")
