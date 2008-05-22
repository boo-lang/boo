"""
BCW0014-3.boo(10,19): BCW0014: WARNING: Private method 'Test.NeverUsed' is never used.
BCW0014-3.boo(13,5): BCW0014: WARNING: Private field 'Test._neverUsed' is never used.
"""
final class Test: #static class are sealed

	def DoSomething():
		pass

	protected def NeverUsed():
		pass

	_neverUsed = 0

