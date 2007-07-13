"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class LenModule(object):

	private static def Main(argv as (string)) as void:
		l = []
		t = (of object: ,)
		s = ''
		o = ('' as object)
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
o = "" as object
print(len(l))
print(len(t))
print(len(s))
print(len(o))
