"""
int: 42
string: void
"""
class Foo:
	private def foo(i as int):
		print "int:", i
		
	private def foo(s as string):
		print "string:", s
		
	def dispatch(o):
		(self as duck).foo(o)
		
f = Foo()
f.dispatch(42)
f.dispatch("void")
