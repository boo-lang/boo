"""
Foo.Dispose
"""
import System

class Foo(IDisposable):
	def Dispose():
		print("Foo.Dispose")

def dispose(obj as IDisposable):
	obj.Dispose()
	
dispose(Foo())

