"""
public final transient class Assert1Module(System.Object):

	private static def __Main__(argv as (System.String)) as System.Void:
		unless (true and false):
			raise Boo.AssertionFailedException('(true and false)')

	private def constructor():
		super()
"""
assert true and false
