"""
BCE0169-1.boo(14,9): BCE0169: `BarOMGHeSaidBarInsteadOfFooIWillDiiiiie' in explicit interface declaration is not a member of interface `IFoo'.
BCE0169-1.boo(16,9): BCE0169: `Foo' in explicit interface declaration is not a member of interface `IFoo'.
"""
macro disableBCW0011:
	Context.Parameters.DisableWarning("BCW0011")
disableBCW0011 #Foo not implemented, stub created...


interface IFoo:
	def Foo()

class Foo(IFoo):
	def IFoo.BarOMGHeSaidBarInsteadOfFooIWillDiiiiie():
		pass
	def IFoo.Foo(evilArgument as bool):
		pass
