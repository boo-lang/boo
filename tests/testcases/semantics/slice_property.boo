"""
import System.Collections

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Slice_propertyModule(object):

	private static def Main(argv as (string)) as void:
		h = Hashtable()
		h.set_Item('foo', 'bar')
		Boo.Lang.Builtins.print(h.get_Item('foo'))

	private def constructor():
		super()
"""
import System.Collections

h = Hashtable()
h["foo"] = "bar"
print(h["foo"])
