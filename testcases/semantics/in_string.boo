"""
public final transient class In_stringModule(System.Object):

	private static def __Main__() as System.Void:
		Boo.Lang.Builtins.print(Boo.Lang.RuntimeServices.op_Member('f', 'foo'))
		Boo.Lang.Builtins.print(Boo.Lang.RuntimeServices.op_NotMember('f', 'foo'))

	private def constructor():
		super()
"""
print("f" in "foo")
print("f" not in "foo")
