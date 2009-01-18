"""
Hello
"""
import Boo.Lang.Compiler

macro foo:
	yield [|
		class Foo:
			def constructor():
				$(foo.Body)
	|]
	
macro bar:
	yield [|
		class Bar:
			def constructor():
				$(bar.Body)
	|]
	
macro baz:
	yield [|
		class Baz:
			def constructor():
				$(baz.Body)
	|]
	
foo:
	bar:
		baz:
			print "Hello"
			
Foo.Bar.Baz()
