"""
import System.Collections

public final transient class Slice_propertyModule(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		h = System.Collections.Hashtable()
		h.set_Item('foo', 'bar')
		Boo.Lang.Builtins.print(h.get_Item('foo'))

	private def constructor():
		super()
"""
import System.Collections

h = Hashtable()
h["foo"] = "bar"
print(h["foo"])
