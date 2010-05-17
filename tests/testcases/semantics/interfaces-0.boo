"""
public class Disposable(object, System.IDisposable):

	public def constructor():
		super()

	public virtual def Dispose() as void:
		raise System.NotImplementedException()
"""
class Disposable(System.IDisposable):
	pass
