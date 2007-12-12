"""
import System.IO

[System.Runtime.CompilerServices.CompilerGlobalScopeAttribute]
public final transient class Using1Module(object):

	private static def Main(argv as (string)) as void:
		__using2__ = ((f1 = File.OpenText('using0.boo')) as System.IDisposable)
		try:
			__using1__ = ((f2 = File.OpenText('using1.boo')) as System.IDisposable)
			try:
				Boo.Lang.Builtins.print(f2.ReadLine())
			ensure:
				if __using1__ is not null:
					__using1__.Dispose()
					__using1__ = null
			Boo.Lang.Builtins.print(f1.ReadLine())
		ensure:
			if __using2__ is not null:
				__using2__.Dispose()
				__using2__ = null

	private def constructor():
		super()

"""
import System.IO

using f1 = File.OpenText('using0.boo'):
	using f2 = File.OpenText('using1.boo'):
		print(f2.ReadLine())
	print(f1.ReadLine())
