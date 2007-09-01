"""
public interface IFoo:

	def Bar() as object

	def Baz() as object

	event Zeng as System.EventHandler

	Ding as string:
		get
		set

public abstract class Foo(object, IFoo):

	public event Zeng as System.EventHandler

	public virtual def Bar() as object:
		return 'Foo.Bar'

	public def constructor():
		super()

	protected ___Zeng as System.EventHandler

	public virtual def Baz() as object:
		raise System.NotImplementedException()

	public abstract Ding as string:
		public abstract get:
			pass
		public abstract set:
			pass
"""
interface IFoo:
	def Bar() as object
	def Baz() as object
	event Zeng as System.EventHandler
	Ding as string:
		get
		set
	
class Foo(IFoo):
	event Zeng as System.EventHandler
	
	def Bar():
		return "Foo.Bar"
		
