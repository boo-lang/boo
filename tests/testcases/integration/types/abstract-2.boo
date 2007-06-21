import NUnit.Framework

abstract class FooProtocol:
	abstract def Bar() as string:
		pass
		
class Foo1(FooProtocol):
	pass
	
class Foo2(Foo1):
	def Bar():
		return "Foo2.Bar"

foo as FooProtocol = Foo2()
Assert.AreEqual("Foo2.Bar", foo.Bar())
