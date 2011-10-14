interface IFoo:
	def Bar() as string
		
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	def Bar():
		return "Foo.Bar"

foo as IFoo = Foo()
assert "Foo.Bar" == foo.Bar()
