"""
public final transient Stringslice0Module(System.Object):

	private static def __Main__() as System.Void
		s = 'foo'
		Boo.Lang.Builtins.print(s.get_Chars(0))
		Boo.Lang.Builtins.print(s.Substring(1))
		Boo.Lang.Builtins.print(Boo.Lang.RuntimeServices.Mid(s, 0, 1))

	private def constructor():
		super()
	
"""
s = "foo"
print(s[0])
print(s[1:])
print(s[0:1])
