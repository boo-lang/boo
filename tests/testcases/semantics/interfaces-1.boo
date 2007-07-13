"""
public interface IFoo:

	public abstract def Bar() as object:
		pass

public class Foo(object, IFoo):

	public virtual def Bar() as object:
		return 'Foo.Bar'

	public def constructor():
		super()
"""
interface IFoo:
	def Bar() as object
	
class Foo(IFoo):
	def Bar():
		return "Foo.Bar"
