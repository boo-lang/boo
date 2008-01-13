"""
BCW0014-1.boo(9,17): BCW0014: WARNING: Private method 'Test.NeverUsed' is never used.
BCW0014-1.boo(13,13): BCW0014: WARNING: Private field 'Test._neverUsed' is never used.
"""
class Test:
	def NeverUsedPublic():
		pass

	private def NeverUsed():
		pass

	public neverUsedPublic = 0
	private _neverUsed = 0
