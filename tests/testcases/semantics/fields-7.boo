"""
public class B(System.Object):

	public def constructor():
		super()
		self.___initializer()
		System.Console.WriteLine(self.a)
		System.Console.WriteLine(self.b)

	protected b as System.String

	protected a as System.String

	def ___initializer() as System.Void:
		self.b = 'foo'
		self.a = 'bar'
"""
class B:
	def constructor():
		print a
		print b
		
	b = "foo"
	a = "bar"
