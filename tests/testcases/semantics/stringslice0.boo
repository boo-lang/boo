"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Stringslice0Module(object):

	private static def Main(argv as (string)) as void:
		s = 'foo'
		Boo.Lang.Builtins.print(s.get_Chars(0))
		Boo.Lang.Builtins.print(s.Substring(1))
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.Mid(s, 0, 1))
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.Mid(s, 0, -1))
		Boo.Lang.Builtins.print(s.Substring(0))

	private def constructor():
		super()
	
"""
s = "foo"
print(s[0])
print(s[1:])
print(s[0:1])
print(s[:-1])
print(s[:])
