import System
import NUnit.Framework

callable Function(item) as object

def identity(item):
	return item
	
d as object = cast(Function, identity)
f as Function = d
Assert.AreEqual("foo", f("foo"))
