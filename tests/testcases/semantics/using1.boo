"""
import System.IO

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Using1Module(object):

	private static def Main(argv as (string)) as void:
		\$using\$disposable\$2 = ((f1 = File.OpenText('using0.boo')) as System.IDisposable)
		try:
			\$using\$disposable\$1 = ((f2 = File.OpenText('using1.boo')) as System.IDisposable)
			try:
				Boo.Lang.Builtins.print(f2.ReadLine())
			ensure:
				if \$using\$disposable\$1 is not null:
					\$using\$disposable\$1.Dispose()
					\$using\$disposable\$1 = null
			Boo.Lang.Builtins.print(f1.ReadLine())
		ensure:
			if \$using\$disposable\$2 is not null:
				\$using\$disposable\$2.Dispose()
				\$using\$disposable\$2 = null

	private def constructor():
		super()

"""
import System.IO

using f1 = File.OpenText('using0.boo'):
	using f2 = File.OpenText('using1.boo'):
		print(f2.ReadLine())
	print(f1.ReadLine())
