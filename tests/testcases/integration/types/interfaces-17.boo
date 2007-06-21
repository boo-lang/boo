import NUnit.Framework

interface IFoo:
	def Bar() as string
		
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	def Bar():
		return "Foo.Bar"

foo as IFoo = Foo()
Assert.AreEqual("Foo.Bar", foo.Bar())
