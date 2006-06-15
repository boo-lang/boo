"""
1, 2, 3
"""
import Boo.Lang.Runtime

class My:
	[Extension]
	static def print(a as System.Array):
		print join(a, ', ')
			
try:
	RuntimeServices.RegisterExtensions(My)
	
	a as duck = (1, 2, 3)	
	a.print()
ensure:
	RuntimeServices.UnRegisterExtensions(My)
