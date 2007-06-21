"""
testabstract.foo
concreteone.myclass.doit
"""
abstract class AbstractOne:
	abstract def Foo() as string:
		pass
		
	class MyClass:
		def doit():
			print "abstractone.myclass.doit"

class ConcreteOne:
	virtual def Foo() as string:
		m = MyClass()
		m.doit()

	class MyClass:
		def doit():
			print "concreteone.myclass.doit"

class TestAbstract(AbstractOne):
	override def Foo():
		print "testabstract.foo"

class TestConcrete(ConcreteOne):
	override def Foo():
		super()

t1 = TestAbstract()
t1.Foo()

t2 = TestConcrete()
t2.Foo()
