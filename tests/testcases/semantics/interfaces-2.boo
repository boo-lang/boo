"""
public interface IFoo:

	def Bar() as object

	def Baz() as object

	event Zeng as System.EventHandler

	Ding as string:
		get
		set

public class Foo(object, IFoo):

	public event Zeng as System.EventHandler

	public virtual def Bar() as object:
		return 'Foo.Bar'

	public def constructor():
		super()

	private \$event\$Zeng as System.EventHandler

	public virtual def Baz() as object:
		raise System.NotImplementedException()

	public virtual Ding as string:
		public virtual get:
			raise System.NotImplementedException()
		public virtual set:
			raise System.NotImplementedException()
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
		
