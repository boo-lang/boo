"""
import System.IO

public final transient class Using1Module(System.Object):

	private static def __Main__() as System.Void:
		try:
			f1 = System.IO.File.OpenText('using0.boo')
			try:
				f2 = System.IO.File.OpenText('using1.boo')
				Boo.Lang.Builtins.print(f2.ReadLine())
			ensure:
				if (__disposable__ = f2 as System.IDisposable):
					__disposable__.Dispose()
					__disposable__ = null
			Boo.Lang.Builtins.print(f1.ReadLine())
		ensure:
			if (__disposable__ = f1 as System.IDisposable):
				__disposable__.Dispose()
				__disposable__ = null

	private def constructor():
		super()

"""
import System.IO

using f1 = File.OpenText('using0.boo'):
	using f2 = File.OpenText('using1.boo'):
		print(f2.ReadLine())
	print(f1.ReadLine())
