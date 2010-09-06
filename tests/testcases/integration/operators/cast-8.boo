"""
Dispose
"""
import System

class Foo:
	pass
	
class Bar(Foo, IDisposable):
	def Dispose():
		print "Dispose"

f as Foo
f = Bar()
d = f cast IDisposable
d.Dispose()
