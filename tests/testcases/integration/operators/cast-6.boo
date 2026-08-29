"""
Clone
Dispose
"""
import System

class Foo(ICloneable, IDisposable):
	def Dispose():
		print "Dispose"
		
	def Clone() as object:
		print "Clone"
		return self
		
def clone(cloneable as ICloneable):
	return cloneable.Clone()
	
def dispose(disposable as IDisposable):
	disposable.Dispose()

o as object = Foo()		
c as ICloneable = o
d as IDisposable = o
assert c is o
assert d is o

c = d
d = c
assert c is o
assert d is o

clone(d)
dispose(c)
