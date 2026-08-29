"""
Hello, world!
"""
import Boo.Lang.Compiler.MetaProgramming

code = [|
	namespace Foo
	
	print "Hello, world!"
|]

compile(code).EntryPoint.Invoke(null, (null,))

