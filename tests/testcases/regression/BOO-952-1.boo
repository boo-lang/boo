"""
Pan!
Pan!

Pan!
"""
import System

static class Test:
	public event X as EventHandler

	def Fire():
		X(null, null)

	def WhiteFlag():
		X = null


def Handler(o as object, args as EventArgs):
	print "Pan!"

Test.X += Handler
Test.X += Handler
Test.Fire()

Test.WhiteFlag()
print ""

Test.X += Handler
Test.Fire()

