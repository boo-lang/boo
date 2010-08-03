"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Collection_initializerModule(object):

	private static def Main(argv as (string)) as void:
		l = @((\$collection\$1 = System.Collections.Generic.List[of string]()), \$collection\$1.Add('foo'), \$collection\$1.Add('bar'), \$collection\$1)

	private def constructor():
		super()

"""
l = System.Collections.Generic.List[of string]() { "foo", "bar" }
