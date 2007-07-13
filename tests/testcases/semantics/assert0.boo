"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Assert0Module(object):

	private static def Main(argv as (string)) as void:
		unless true and false:
			raise Boo.Lang.Runtime.AssertionFailedException('assert message')

	private def constructor():
		super()
"""
assert true and false, "assert message"
