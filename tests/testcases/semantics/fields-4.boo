"""
public class Foo(object):

	protected static FinalSolution as string

	protected _some as int

	public def constructor():
		super()
		self._some = 3

	private static def constructor():
		Foo.FinalSolution = '42'
"""

class Foo:
	static FinalSolution = "42"
	_some = 3
