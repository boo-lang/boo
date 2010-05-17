"""
Regular method
"""

class C[of T]:
	def Method(arg as int):
		return "Regular method"

	def Method(arg as T):
		return "Generic mapped method"

c = C[of int]()
print c.Method(42)

