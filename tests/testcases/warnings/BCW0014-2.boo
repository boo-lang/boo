"""
BCW0014-2.boo(12,17): BCW0014: WARNING: Private method 'Test.NeverUsed' is never used.
BCW0014-2.boo(15,18): BCW0014: WARNING: Internal method 'Test.InternalFoo' is never used.
"""
class Test:
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

