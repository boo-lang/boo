# This test tells us whether the warning stays off for overloads a call can
# tell apart.

class Overloads:
	# An exact match wins over one that fills a default.
	def a(x as int):
		pass
	def a(x as int, y as int = 2):
		pass

	# Different first parameter types.
	def b(x as int, y as int = 2):
		pass
	def b(x as string, y as int = 2):
		pass

	# Dropping the default still leaves the second needing two arguments.
	def c(x as int, y as int):
		pass
	def c(x as int, y as int, z as int = 3):
		pass

	# Param array expansion breaks the tie first.
	def d(x as int, *rest as (int)):
		pass
	def d(x as int, y as int = 2):
		pass
