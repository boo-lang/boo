"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Stringslice1Module(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		Boo.Lang.Builtins.print('foo'.Substring(1).get_Chars(0))

	private def constructor():
		super()
"""
print("foo"[1:][0])
