"""
ASDF
"""
class Bravo:
	def Thing():
		return Alpha.Item['asdf']

class Alpha:
	static Item(name) as string:
		get:
			return name.ToString().ToUpper()

print Bravo().Thing()

