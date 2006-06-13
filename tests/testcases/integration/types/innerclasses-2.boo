"""
Outer+Inner.Dispose
"""
import NUnit.Framework

class Outer:
	class Inner(System.IDisposable):
		def Dispose():
			print("${GetType()}.Dispose")

types = typeof(Outer).GetNestedTypes()
Assert.AreEqual(1, len(types))

Assert.AreSame(Outer.Inner, types[0])
Assert.IsTrue(types[0] is Outer.Inner)
Assert.IsTrue(typeof(Outer.Inner).IsNestedPublic)

using inner=Outer.Inner():
	pass
