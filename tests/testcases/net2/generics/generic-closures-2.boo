"""
pass
"""
import System

static class Foo[of T]:
	
	def Bar(baz as T):
		var method = {return baz}
		print method()

Foo[of string].Bar('pass')