"""
public class Foo(object):

	public def Test(arg as string) as bool:
		return Foo.\$re\$1.IsMatch(arg)

	public def Bar() as void:
		re = /bar/

	public def constructor():
		super()

	internal static \$re\$1 as regex

	private static def constructor():
		Foo.\$re\$1 = /foo/
"""
class Foo:
	def Test(arg as string):
		return /foo/.IsMatch(arg)
		
	def Bar():
		re = /bar/ # not cached
