"""
Soviet Russia says hello to Foo!
Soviet Russia says hello to Bar!
Soviet Russia says hello to Zap!
Mission accomplished, both classes and expressions are defeated!
"""

import Boo.Lang.Compiler

macro mixed:
	i = 0
	for typeName in mixed.Arguments:
		yield [|
			class $typeName:
				override def ToString() as string:
					return "Soviet Russia says hello to ${$typeName}!"
		|]
		yield [| print $typeName() |]
		i++

	yield [|
		if $i == 3:
			print "Mission accomplished, both classes and expressions are defeated!"
	|]

mixed Foo, Bar, Zap

