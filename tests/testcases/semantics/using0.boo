"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Using0Module(object):

	private static def Main(argv as (string)) as void:
		\$using\$disposable\$1 = ((f = System.IO.File.OpenText('using0.boo')) as System.IDisposable)
		try:
			Boo.Lang.Builtins.print(f.ReadLine())
		ensure:
			if \$using\$disposable\$1 is not null:
				\$using\$disposable\$1.Dispose()
				\$using\$disposable\$1 = null

	private def constructor():
		super()

"""
using f = System.IO.File.OpenText('using0.boo'):
	print(f.ReadLine())
