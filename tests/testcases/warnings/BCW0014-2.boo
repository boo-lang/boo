"""
BCW0014-2.boo(11,17): BCW0014: WARNING: Private method 'Test.NeverUsed' is never used.
"""
class Test:
	def Use():
		UsedPrivate()

	private def UsedPrivate():
		pass

	private def NeverUsed():
		pass

	[getter(IUseTheField)]
	private _fieldNeverUsedDirectly = 0
