"""
pass
"""
import System

static class Foo:
	
	def Bar[of T](baz as T):
		var method = {return baz}
		print method()

Foo.Bar('pass')