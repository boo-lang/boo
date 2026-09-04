"""
BCE0185-2.boo(15,13): BCE0185: 'Point' has no parameter named 'ex'.
BCE0185-2.boo(16,24): BCE0185: 'StringBuilder' has no parameter named 'nope'.
"""
import System.Text

class Point:
	public X as int
	def constructor(x as int):
		X = x

# A constructor is checked like any other call, and is named for the type it
# builds rather than reported as 'constructor'.
# This test tells us whether a misspelled constructor parameter is refused.
p = Point(ex=1)
sb = StringBuilder(nope=1)
