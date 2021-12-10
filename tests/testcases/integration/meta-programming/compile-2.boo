"""
Hello, world!
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.MetaProgramming

code = [|
	namespace Foo
	
	print "Hello, world!"
|]

compile(code).GetEntryPoint().Invoke(null, (null,))

