"""
Foo says hello!
Bar says hello!
Zap says hello!
"""

import Boo.Lang.Compiler

macro helloClasses:
	for typeName in helloClasses.Arguments:
		yield [|
			class $typeName:
				override def ToString() as string:
					return "${$typeName} says hello!"
		|]

helloClasses Foo, Bar, Zap

print Foo()
print Bar()
print Zap()

