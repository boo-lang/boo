"""
Bill
Bill
"""

// this test makes sure duck types are preserved and recognized in compiled
// assemblies

import Boo.Lang.Compiler.MetaProgramming

module = [|

	import System # force it to be interpreted as module
	
	class Person:
		[property(Name)]
		_name = ''
		
	class Foo:
		static def bar() as duck:
			return Person(Name: 'Bill')
			
		static baz as duck:
			get:
				return Person(Name: 'Bill')
|]

module.Name = "lib"

library = compile(module)

appCode = [|
	class App:
		def Run():
			print bar().Name
			print baz().Name
			
		def bar():
			return Foo.bar()
			
		def baz():
			return Foo.baz
|]

appType = compile(appCode, library)
(appType() as duck).Run()

	

