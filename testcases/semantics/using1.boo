"""
import System.IO

public final transient class Using1Module(System.Object):

	private static def __Main__() as System.Void:
		try:
			f1 = File.OpenText('using0.boo')
			try:
				f2 = File.OpenText('using1.boo')
				print(f2.ReadLine())
			ensure:
				if (__disposable = f2 as System.IDisposable):
					__disposable.Dispose()
					__disposable = null
			print(f1.ReadLine())
		ensure:
			if (__disposable = f1 as System.IDisposable):
				__disposable.Dispose()
				__disposable = null

	private def constructor():
		pass

"""
import System.IO

using f1 = File.OpenText('using0.boo'):
	using f2 = File.OpenText('using1.boo'):
		print(f2.ReadLine())
	print(f1.ReadLine())
