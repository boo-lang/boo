"""
public final transient class In_stringModule(System.Object):

	private static def __Main__() as System.Void:
		print(Boo.Lang.RuntimeServices.StringContains('foo', 'f'))
		print((not Boo.Lang.RuntimeServices.StringContains('foo', 'f')))

	private def constructor():
		super()
"""
print("f" in "foo")
print("f" not in "foo")
