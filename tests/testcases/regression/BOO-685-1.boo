"""
Some name
"""

class Base:
	private parent = object()

class Derived(Base):
	def Method():
		parent = "Some name"
		print parent

d = Derived()
d.Method()
