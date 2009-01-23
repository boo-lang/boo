"""
BCW0025-1.boo(36,9): BCW0025: WARNING: Variable 'i' has the same name as a private field of base type 'Base'. Did you mean to use the field?
BCW0025-1.boo(42,9): BCW0025: WARNING: Variable 'i' has the same name as a private field of base type 'Base'. Did you mean to use the field?
BCW0025-1.boo(43,9): BCW0025: WARNING: Variable 'k' has the same name as a private field of base type 'BaseBase'. Did you mean to use the field?
"""
macro disableBCW0014:
	Context.Parameters.DisableWarning("BCW0014") #unused privates
disableBCW0014

interface IFoo:
	def Foo()

interface IBar:
	def Bar()

class BaseBase:
	private k = 2
	internal o = "o"

class Base(BaseBase,IBar):
	private i = 0
	private j = 1
	n = 1

	virtual def Foo():
		print i

	virtual def Bar():
		print j

	def X():
		print n

class E(Base,IFoo):
	def Foo():
		i = 42 #!
		z = 13
		print i
		print z

	def Bar():
		i = 1 #!
		k = 0 #!
		z = 8
		print "${i}.${k}.${z}"

	def BarExplicit():
		k as int = 0
		print k

	def Baz():
		n = 84
		o = "O"

