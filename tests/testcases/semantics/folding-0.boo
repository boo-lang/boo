"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Folding_0Module(object):

	private static def Main(argv as (string)) as void:
		answer = 42
		TRUE = true
		FALSE = false
		OR0 = false
		OR1 = true
		AND0 = false
		AND1 = true
		PI2 = true

	private def constructor():
		super()
"""

answer = (11 + 10) * 2
TRUE = (12 - 4 == 4 << 1)
FALSE = ((0.5 * 2) > 2.0f)
OR0 = false or false
OR1 = false or 5 == 4+1
AND0 = false and true
AND1 = 8>>1==2+2 and 1L<2.0
PI2 = System.Math.PI >= 3.14

