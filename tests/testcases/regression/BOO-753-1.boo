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
	def constructor():
		super("a", "b")
		print "ho"

Ho()
