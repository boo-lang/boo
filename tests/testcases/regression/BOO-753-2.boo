"""
hey
a b
ho
"""
import System

class Hey:
	def constructor(*foo):
		print "hey"
		print join(foo)

class Ho(Hey):
	def constructor(*foo):
		super(*foo)
		print "ho"

Ho("a", "b")
