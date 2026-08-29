"""
null reference
"""
class Person:
	Name as string:
		get:
			return null

try:
	# make sure the getter is typed string by
	# calling Trim
	print(Person().Name.Trim())
except x as System.NullReferenceException:
	print("null reference")
