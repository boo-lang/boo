"""
3
"""
import Boo.Lang.Runtime

class My:
	[Extension]
	static length[a as System.Array]:
		get:
			return a.Length
			
try:
	RuntimeServices.RegisterExtensions(My)
	
	a as duck = (1, 2, 3)
	print a.length
ensure:
	RuntimeServices.UnRegisterExtensions(My)
