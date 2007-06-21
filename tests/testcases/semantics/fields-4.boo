"""
public class Foo(System.Object):

	protected static FinalSolution as System.String

	protected _some as System.Int32

	public def constructor():
		super()
		self.___initializer()

	public static def constructor():
		Foo.FinalSolution = '42'

	def ___initializer() as System.Void:
		self._some = 3
"""
class Foo:
	static FinalSolution = "42"
	_some = 3
