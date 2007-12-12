"""
before block
1: constructor
2: constructor
inside block
2: Dispose
1: Dispose
after block
"""
import NUnit.Framework

class Disposable(System.IDisposable):
	_tag
	def constructor(tag):
		_tag = tag
		print("${tag}: constructor")
		
	def Dispose():
		print("${_tag}: Dispose")

print("before block")
using Disposable(1), d1=Disposable(2):
	print("inside block")
print("after block")
Assert.IsNotNull(d1)
