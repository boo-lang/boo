"""
IFoo.Foo
"""
interface IFoo:

	def Foo()
	
class Foo(IFoo):
	def Foo():
		print("IFoo.Foo")

def fight(foo as IFoo):
	foo.Foo()

fight(Foo())
