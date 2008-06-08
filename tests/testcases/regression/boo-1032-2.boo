"""
Foo
IFoo1.Foo
"""

interface IFoo1:
	def Foo() as string: 
		pass

interface IFoo2:
	def Foo() as string:
		pass

class Bar(IFoo1, IFoo2):
	def IFoo1.Foo():
		return "IFoo1.Foo"

	def Foo():
		return "Foo"
bar = Bar()
print bar.Foo()

ifoo1 as IFoo1 = bar
print ifoo1.Foo()
