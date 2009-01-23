"""
foo.bar
yummy
foo.bar
yummy
yummy
"""
namespace NestedMacros

import Boo.Lang.Compiler.MetaProgramming

macro foo:
	macro bar:
		yield [| print "foo.bar" |]
	yield foo.Body

macro choco:
	macro bar:
		yield [| print "yummy" |]
		if choco.Arguments.Count > 0:
			yield [| print "yummy" |]
	yield choco.Body

foo:
	bar # foo.bar
choco:
	bar # yummy

code = [|
	import NestedMacros
	foo:
		bar # foo.bar
	choco 2:
		bar # yummy
|]
result = compile(code, typeof(FooMacro).Assembly)
result.EntryPoint.Invoke(null, (null,))

