"""
[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Using0Module(object):

	private static def Main(argv as (string)) as void:
		__using1__ = ((f = System.IO.File.OpenText('using0.boo')) as System.IDisposable)
		try:
			Boo.Lang.Builtins.print(f.ReadLine())
		ensure:
			if __using1__ is not null:
				__using1__.Dispose()
				__using1__ = null

	private def constructor():
		super()

"""
using f = System.IO.File.OpenText('using0.boo'):
	print(f.ReadLine())
