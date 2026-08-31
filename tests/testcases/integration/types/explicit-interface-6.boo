"""
typed
typed/explicit
"""

interface IFoo:
	def Bar(x as int) as object

class C(IFoo):
	def Bar(x as int) as string:
		return "typed"

	# The explicit member reaches the one of its own name, which is the shape
	# a generated visitor uses: a typed method plus a forwarding one.
	def IFoo.Bar(x as int) as object:
		return Bar(x) + "/explicit"

c = C()
print c.Bar(1)
print cast(IFoo, c).Bar(1)
