"""
public interface IFoo:

	public abstract def Bar() as System.Object:
		pass

	public abstract def Baz() as System.Object:
		pass

public abstract class Foo(System.Object, IFoo):

	public virtual def Bar() as System.Object:
		return 'Foo.Bar'

	public def constructor():
		super()
"""
interface IFoo:
	def Bar() as object
	def Baz() as object
	
abstract class Foo(IFoo):
	def Bar():
		return "Foo.Bar"
