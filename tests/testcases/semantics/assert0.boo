"""
[Boo.Lang.BooModuleAttribute]
public final transient class Assert0Module(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		unless (true and false):
			raise Boo.AssertionFailedException('assert message')

	private def constructor():
		super()
"""
assert true and false, "assert message"
