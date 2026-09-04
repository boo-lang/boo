"""
BCW0033-1.boo(13,9): BCW0033: WARNING: A 1-argument call cannot tell 'f(int, int)' from 'f(int, int, int)', since defaults fill the parameters it leaves out.
BCW0033-1.boo(21,9): BCW0033: WARNING: A 1-argument call cannot tell 'constructor(int, int)' from 'constructor(int, string)', since defaults fill the parameters it leaves out.
"""
# Both overloads accept one argument and declare int first, so a one argument
# call matches both equally.
# This test tells us whether the pair is reported where it is declared.

class Overloads:
	def f(a as int, b as int = 2):
		pass

	def f(a as int, b as int = 2, c as int = 3):
		pass

# Constructors are overloaded the same way.
class Point:
	def constructor(x as int, y as int = 0):
		pass

	def constructor(x as int, label as string = ""):
		pass
