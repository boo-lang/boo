"""
[Boo.Lang.ModuleAttribute]
public final transient class Using0Module(System.Object):

	private static def Main(argv as (System.String)) as System.Void:
		try:
			f = System.IO.File.OpenText('using0.boo')
			Boo.Lang.Builtins.print(f.ReadLine())
		ensure:
			if __disposable__ = (f as System.IDisposable):
				__disposable__.Dispose()
				__disposable__ = null

	private def constructor():
		super()

"""
using f = System.IO.File.OpenText('using0.boo'):
	print(f.ReadLine())
