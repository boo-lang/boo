"""
public interface IFoo:

	public abstract def Bar() as System.Object:
		pass

public class Foo(System.Object, IFoo):

	public virtual def Bar() as System.Object:
		return 'Foo.Bar'

	public def constructor():
		super()
"""
interface IFoo:
	def Bar() as object
	
class Foo(IFoo):
	def Bar():
		return "Foo.Bar"
