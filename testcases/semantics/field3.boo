"""
public class Foo(System.Object):

	protected static FinalSolution as System.String

	protected _some as System.Int32

	public def constructor():
		super()
		self._some = 3

	public static def constructor():
		Foo.FinalSolution = '14'
"""
class Foo:
	static FinalSolution = "14"
	_some = 3
