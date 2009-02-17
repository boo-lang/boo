"""
Hello!
"""
import Boo.Lang.Compiler.MetaProgramming

libModule = [|
	namespace Lib
	
	def hello():
		print "Hello!"		
|]
libModule.Namespace = null

lib = compile(libModule)

client = [|
	namespace Client
	
	hello()
|]
compile(client, lib).EntryPoint.Invoke(null, (null,))


