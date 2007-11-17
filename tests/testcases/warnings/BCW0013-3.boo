"""
BCW0013-3.boo(6,29): BCW0013: WARNING: 'green' on static type 'Foo' is redundantly marked static. All members of static types are automatically assumed to be static.
"""
class Test:
	static class Foo:
		public static final green = -4
	
	private static def DoTheTest():
		print Foo.green
	
	def Bar():
		DoTheTest()
