"""
FOO
foo
"""
def ToUpper(s as string):
	return s.ToUpper()
	
def ToLower(s as string):
	return s.ToLower()

def Transform(s, upper as bool):
	if upper:
		t = ToUpper
	else:
		t = ToLower
	return t(s)

a = "Foo"
print(Transform(a, true))
print(Transform(a, false))

