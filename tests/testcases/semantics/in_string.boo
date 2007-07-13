"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class In_stringModule(object):

	private static def Main(argv as (string)) as void:
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.op_Member('f', 'foo'))
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.op_NotMember('f', 'foo'))

	private def constructor():
		super()
"""
print("f" in "foo")
print("f" not in "foo")
