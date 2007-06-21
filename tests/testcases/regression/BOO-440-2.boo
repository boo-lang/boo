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
			
	def Run():
		print Demo["foo"]
		print Demo

Second().Run()
