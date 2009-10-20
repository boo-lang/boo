"""
BCW0014-2.boo(12,17): BCW0014: WARNING: Private method 'Test.NeverUsed' is never used.
BCW0014-2.boo(15,18): BCW0014: WARNING: Internal method 'Test.InternalFoo' is never used.
BCW0014-2.boo(35,13): BCW0014: WARNING: Private method 'BCW0014_2Module.ModulePrivate' is never used.
"""
class Test(ITest):
	def Use():
		UsedPrivate()

	private def UsedPrivate():
		pass
	private def NeverUsed():
		pass

	internal def InternalFoo():
		pass

	[getter(IUseTheField)]
	private _fieldNeverUsedDirectly = 0

	def ITest.SomeMethod():
		pass

	ITest.SomeProperty as bool:
		get:
			return false

interface ITest:
	def SomeMethod():
		pass

	SomeProperty as bool:
		get

private def ModulePrivate():
	pass

private def Main(argv as (string)): #NO WARN
	pass

