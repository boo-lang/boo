"""
drawrof
"""
namespace ClrExtensions

import Boo.Lang.Compiler.MetaProgramming

[System.Runtime.CompilerServices.ExtensionAttribute]
def Reverse(this as string):
	return join(reversed(this))

code = [|
	import ClrExtensions
	
	forward = "forward"
	reverse = forward.Reverse()

	print reverse
|]
compile(code).EntryPoint.Invoke(null, (null,))