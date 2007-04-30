"""
public interface IFoo:

	public abstract def Bar() as System.Object:
		pass

	public abstract def Baz() as System.Object:
		pass

public class Foo(System.Object, IFoo):

	public virtual def Bar() as System.Object:
		return 'Foo.Bar'

	public def constructor():
		super()

	public virtual def Baz() as System.Object:
		raise System.NotImplementedException()
"""
interface IFoo:
	def Bar() as object
	def Baz() as object
	
class Foo(IFoo):
	def Bar():
		return "Foo.Bar"
