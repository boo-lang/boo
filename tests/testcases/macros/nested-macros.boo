"""
foo.bar
yummy
foo.bar
yummy
"""
namespace NestedMacros

import Boo.Lang.Compiler.MetaProgramming

macro foo:
	macro bar:
		yield [| print "foo.bar" |]
	yield foo.Block

macro choco:
	macro bar:
		yield [| print "yummy" |]
	yield choco.Block
	
foo:
	bar # foo.bar
choco:
	bar # yummy
	
code = [|
	import NestedMacros
	foo:
		bar # foo.bar
	choco:
		bar # yummy
|]
result = compile(code, typeof(FooMacro).Assembly)
result.EntryPoint.Invoke(null, (null,))
