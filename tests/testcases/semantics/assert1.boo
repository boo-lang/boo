"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Assert1Module(object):

	private static def Main(argv as (string)) as void:
		unless false:
			raise Boo.Lang.Runtime.AssertionFailedException('true and false')

	private def constructor():
		super()
"""
assert true and false
