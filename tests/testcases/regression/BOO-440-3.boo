"""
foo bar
zeng
"""
class First:
	virtual Demo(s as string) as string:
		set:
			return null

class Second(First):
	override Demo(s as string):
		set:
			print s, value

	Demo:
		set:
			print value
			
	def Run():
		Demo["foo"] = "bar"
		Demo = "zeng"

Second().Run()
