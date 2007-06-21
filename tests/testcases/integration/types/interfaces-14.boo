"""
before
Dispose(True)
after
Dispose(False)
"""
import System

class Disposable(IDisposable):
	def Dispose():
		Dispose(true)
		
	def Dispose(flag as bool):
		print("Dispose(${flag})")
		
print("before")
using d=Disposable():
	pass
print("after")	
d.Dispose(false)
