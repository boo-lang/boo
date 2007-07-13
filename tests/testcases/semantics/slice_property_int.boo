"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Slice_property_intModule(object):

	private static def Main(argv as (string)) as void:
		l = ['foo']
		l.set_Item(0, 'bar')
		Boo.Lang.Builtins.print(l.get_Item(0))

	private def constructor():
		super()
"""
l = ["foo"]
l[0] = "bar"
print(l.get_Item(0))
