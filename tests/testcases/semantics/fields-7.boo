"""
public class B(object):

	public def constructor():
		super()
		self.$initializer$()
		System.Console.WriteLine(self.a)
		System.Console.WriteLine(self.b)

	protected b as string

	protected a as string

	def $initializer$() as void:
		self.b = 'foo'
		self.a = 'bar'
"""
class B:
	def constructor():
		print a
		print b
		
	b = "foo"
	a = "bar"
