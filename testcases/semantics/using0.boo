"""
public final transient class Using0Module(System.Object):

	private static def __Main__() as System.Void:
		try:
			f = System.IO.File.OpenText('using0.boo')
			print(f.ReadLine())
		ensure:
			if (__disposable = f as System.IDisposable):
				__disposable.Dispose()
				__disposable = null

	private def constructor():
		pass

"""
using f = System.IO.File.OpenText('using0.boo'):
	print(f.ReadLine())
