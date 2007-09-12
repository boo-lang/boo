"""
public class Foo(object):

	protected static FinalSolution as string

	protected _some as int

	public def constructor():
		super()
		self.$initializer$()

	public static def constructor():
		Foo.FinalSolution = '42'

	def $initializer$() as void:
		self._some = 3
"""
class Foo:
	static FinalSolution = "42"
	_some = 3
