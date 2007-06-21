"""
FOO
it works
"""
class First:
	virtual Demo(s as string):
		get:
			return null

class Second(First):
	override Demo(s as string):
		get:
			return s.ToUpper()

	Demo:
		get:
			return "it works"
			
			
f as First = Second()
print f.Demo["foo"]
print cast(Second, f).Demo
