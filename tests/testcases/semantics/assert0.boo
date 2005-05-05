"""
[Boo.Lang.ModuleAttribute]
public final transient class Assert0Module(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		unless (true and false):
			raise Boo.Lang.Runtime.AssertionFailedException('assert message')

	private def constructor():
		super()
"""
assert true and false, "assert message"
