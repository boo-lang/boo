"""
[Boo.Lang.BooModuleAttribute]
public final transient class Hash0Module(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		d = { 'foo': 'bar' }
		Boo.Lang.Builtins.print(Boo.Lang.RuntimeServices.op_Member('foo', d))

	private def constructor():
		super()
"""
d = { "foo": "bar" }
print("foo" in d)
