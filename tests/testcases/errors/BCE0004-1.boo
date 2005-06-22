"""
BCE0004-1.boo(10,18): BCE0004: Ambiguous reference 'Foo': Ambiguous.Foo(System.Int32), Ambiguous.Foo().
"""
class Ambiguous:
	def Foo(a as int):
		pass
	def Foo():
		pass
		
fn = Ambiguous().Foo
