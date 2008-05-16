"""
System.Int32
System.String
"""

interface IFoo:
	def Foo[of T]()

class FooImpl(IFoo):
	def Foo[of T]():
		print typeof(T)

foo as IFoo = FooImpl()
foo.Foo[of int]()
foo.Foo[of string]()

