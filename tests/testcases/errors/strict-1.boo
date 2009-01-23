"""
strict-1.boo(15,7): BCE0120: 'Foo.Bar' is inaccessible due to its protection level.
"""
import Boo.Lang.Compiler

macro setStrict:
	Context.Parameters.Strict = true

setStrict

class Foo:
	def Bar():
		pass

Foo().Bar()

