import NUnit.Framework

interface IFoo:
	pass
	
interface IBar(IFoo):
	pass
	
class Foo(IBar):
	pass

foo = Foo()
Assert.IsTrue(foo isa IBar)
Assert.IsTrue(foo isa IFoo)
