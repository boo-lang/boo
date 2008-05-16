"""
42
"""

import System.Collections.Generic

class Foo:
	def Bar(x as int):
		print x*2

	def Do():
		d = Dictionary[of int, callable(int)]()
		d.Add(1, Bar)
		d[1](21)

Foo().Do()

