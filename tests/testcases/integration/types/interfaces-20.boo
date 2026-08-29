"""
foo
bar
"""
interface IFoo:
	def Foo()
	
class Base:
	pass
	
class Foo(Base, IFoo):
	def Foo():
		print "foo"
		
class Bar(Base, IFoo):
	def Foo():
		print "bar"

base as Base
base = Foo()

foo as IFoo = base
foo.Foo()

foo = Bar()
base = foo

foo.Foo()
