"""
public interface IFoo:

	public abstract def Bar() as object:
		pass

	public abstract def Baz() as object:
		pass

public class Foo(object, IFoo):

	public virtual def Bar() as object:
		return 'Foo.Bar'

	public def constructor():
		super()

	public virtual def Baz() as object:
		raise System.NotImplementedException()
"""
interface IFoo:
	def Bar() as object
	def Baz() as object
	
class Foo(IFoo):
	def Bar():
		return "Foo.Bar"
