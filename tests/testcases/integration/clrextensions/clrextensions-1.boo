"""
drawrof
"""
import System
import Boo.Lang.Compiler.MetaProgramming

if Type.GetType("System.Runtime.CompilerServices.ExtensionAttribute") is null:
	print "drawrof"
	return

library = [|
	namespace ClrExtensions
	
	[System.Runtime.CompilerServices.ExtensionAttribute]
	def Reverse(this as string):
		return join(reversed(this))
|]

code = [|
	import ClrExtensions
	
	forward = "forward"
	reverse = forward.Reverse()

	print reverse
|]
compile(code, compile(library)).EntryPoint.Invoke(null, (null,))