"""
public class Foo(object):

	public def constructor():
		super()
		self.a = 1
		self.d = 'abc'
		System.Console.WriteLine(self.a)
		System.Console.WriteLine(self.b)
		System.Console.WriteLine(self.c)
		System.Console.WriteLine(self.d)

	protected a as int

	protected b as int

	protected c as string

	protected d as string

	protected x as single
"""
class Foo:
	def constructor():
		print a
		print b
		print c
		print d

	a = 1
	b = 0
	c as string = null
	d = "abc"
	x = 0.0f

