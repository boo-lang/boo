"""
[Boo.Lang.ModuleAttribute]
public final transient class Slice_property_intModule(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		l = ['foo']
		l.set_Item(0, 'bar')
		Boo.Lang.Builtins.print(l.get_Item(0))

	private def constructor():
		super()
"""
l = ["foo"]
l[0] = "bar"
print(l.get_Item(0))
