"""
drawrof
citats
"""
import System
import Boo.Lang.Compiler.MetaProgramming

extensionAttribute = Type.GetType("System.Runtime.CompilerServices.ExtensionAttribute, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
if extensionAttribute is null:
	print "drawrof"
	print "citats"
	return

library = [|
	namespace ClrExtensions
	
	[System.Runtime.CompilerServices.ExtensionAttribute]
	def Reverse(this as string):
		return join(reversed(this), '')
|]

code = [|
	import ClrExtensions
	
	forward = "forward"
	reverse = forward.Reverse()

	print reverse

	reverse = Reverse("static")

	print reverse
|]
compile(code, compile(library, extensionAttribute.Assembly)).EntryPoint.Invoke(null, (null,))

