"""
foo
bar
"""
import System.Collections.Generic

class Test:
	dict as Dictionary[of string, callable()] 
	
	def Run():
		dict = Dictionary[of string, callable()]() {
			"foo": { print "foo" },
			"bar": { print "bar" },
		}
		dict["foo"]()
		dict["bar"]()
		
Test().Run()
