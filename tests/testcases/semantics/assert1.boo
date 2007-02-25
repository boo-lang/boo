"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Assert1Module(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		unless true and false:
			raise Boo.Lang.Runtime.AssertionFailedException('true and false')

	private def constructor():
		super()
"""
assert true and false
