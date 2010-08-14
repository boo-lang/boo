"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Hash_initializerModule(object):

	private static def Main(argv as (string)) as void:
		d = @((\$collection\$1 = System.Collections.Generic.Dictionary[of string, string]()), \$collection\$1.Add('foo', 'bar'), \$collection\$1)

	private def constructor():
		super()

"""
d = System.Collections.Generic.Dictionary[of string, string]() { "foo": "bar" }
