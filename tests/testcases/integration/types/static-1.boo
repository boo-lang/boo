"""
-4
"""
class Test:
	static class Foo:
		public final green = -4
	
	private static def DoTheTest():
		print Foo.green
	
	def Bar():
		DoTheTest()

Test().Bar()