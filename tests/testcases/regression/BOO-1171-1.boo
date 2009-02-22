"""
foo
"""
import System
import Boo.Lang.Compiler.MetaProgramming

library = [|
	interface ISetter:
		Object as object:
			set
|]

code = [|
	class Setter (ISetter):
		def constructor():
			Object = null

		Object as object:
			set:
				print "foo"
|]

Activator.CreateInstance(compile(code, compile(library).Assembly))

