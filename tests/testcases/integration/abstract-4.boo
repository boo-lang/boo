import BooCompiler.Tests

class Concrete(AnotherAbstractClass):

	override def Foo():
		return "Concrete.Foo"
		
	override def Bar():
		return "Concrete.Bar"
	
c = Concrete()
assert "Concrete.Foo" == c.Foo()
assert "Concrete.Bar" == c.Bar()
