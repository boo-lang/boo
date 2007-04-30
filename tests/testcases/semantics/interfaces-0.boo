"""
public class Disposable(System.Object, System.IDisposable):

	public def constructor():
		super()

	public virtual def Dispose() as System.Void:
		raise System.NotImplementedException()
"""
class Disposable(System.IDisposable):
	pass
