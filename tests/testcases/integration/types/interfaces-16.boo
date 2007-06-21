import NUnit.Framework

interface IFoo:
	def Bar() as string
		
class Foo1(IFoo):
	pass
	
class Foo2(Foo1):
	def Bar():
		return "Foo2.Bar"

foo as IFoo = Foo2()
Assert.AreEqual("Foo2.Bar", foo.Bar())
