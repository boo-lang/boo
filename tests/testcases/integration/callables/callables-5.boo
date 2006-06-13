"""
FOO
foo
"""
def ToUpper(s as string):
	return s.ToUpper()
	
def ToLower(s as string):
	return s.ToLower()

def Select(upper as bool):	
	return ToUpper if upper
	return ToLower

a = "Foo"
print(Select(true)(a))
print(Select(false)(a))
