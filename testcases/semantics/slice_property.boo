"""
import System.Collections

public final transient class Slice_propertyModule(System.Object):

	private static def __Main__() as System.Void:
		h = Hashtable()
		h.set_Item('foo', 'bar')
		print(h.get_Item('foo'))

	private def constructor():
		super()
"""
import System.Collections

h = Hashtable()
h["foo"] = "bar"
print(h["foo"])
