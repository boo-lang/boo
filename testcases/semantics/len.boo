"""
public final transient class LenModule(System.Object):

	private static def __Main__() as System.Void:
		l = []
		t = (,)
		s = ''
		o = '' as System.Object
		print(l.get_Count())
		print(t.get_Length())
		print(s.get_Length())
		print(Boo.Lang.RuntimeServices.Len(o))

	private def constructor():
		super()
"""
l = []
t = (,)
s = ""
o = "" as System.Object
print(len(l))
print(len(t))
print(len(s))
print(len(o))
