"""
public class Foo(System.Object):

	protected static FinalSolution as System.String

	protected _some as System.Int32

	public def constructor():
		super()
		self.___initializer()

	static def ___static_initializer() as System.Void:
		Foo.FinalSolution = '42'

	public static def constructor():
		Foo.___static_initializer()

	def ___initializer() as System.Void:
		self._some = 3
"""
class Foo:
	static FinalSolution = "42"
	_some = 3
