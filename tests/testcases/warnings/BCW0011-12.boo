"""
"""

abstract class AbstractFoo:
	abstract Foo as int:
		get:
			pass

class Foo (AbstractFoo):
	Foo as int:
		get:
			return 0
		set:
			print "set"
