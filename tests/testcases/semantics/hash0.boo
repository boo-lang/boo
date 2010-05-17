"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Hash0Module(object):

	private static def Main(argv as (string)) as void:
		d = { 'foo': 'bar' }
		Boo.Lang.Builtins.print(Boo.Lang.Runtime.RuntimeServices.op_Member('foo', d))

	private def constructor():
		super()
"""
d = { "foo": "bar" }
print("foo" in d)
