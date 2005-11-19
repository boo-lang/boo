"""
[Boo.Lang.ModuleAttribute]
public final transient class LenModule(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		l = []
		t = (of System.Object: ,)
		s = ''
		o = ('' as System.Object)
		Boo.Lang.Builtins.print(l.get_Count())
		Boo.Lang.Builtins.print(t.get_Length())
		Boo.Lang.Builtins.print(s.get_Length())
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.Len(o))

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
