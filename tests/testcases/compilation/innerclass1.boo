"""
1
Outer+Inner
True
True
Outer+Inner.Dispose
"""
class Outer:
	class Inner(System.IDisposable):
		def Dispose():
			print("${GetType()}.Dispose")

types = typeof(Outer).GetNestedTypes()
print(len(types))
print(types[0])
print(types[0] is Outer.Inner)
print(typeof(Outer.Inner).IsNestedPublic)

using inner=Outer.Inner():
	pass
