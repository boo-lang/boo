"""
public final transient class Hash0Module(System.Object):

	private static def __Main__() as System.Void:
		d = { 'foo': 'bar' }
		print(d.Contains('foo'))

	private def constructor():
		super()
"""
d = { "foo": "bar" }
print("foo" in d)
