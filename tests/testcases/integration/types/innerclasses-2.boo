"""
Outer+Inner.Dispose
"""
class Outer:
	class Inner(System.IDisposable):
		def Dispose():
			print("$(GetType()).Dispose")

types = typeof(Outer).GetNestedTypes()
assert 1 == len(types)

assert Outer.Inner is types[0]
assert types[0] is Outer.Inner
assert typeof(Outer.Inner).IsNestedPublic

using inner=Outer.Inner():
	pass
