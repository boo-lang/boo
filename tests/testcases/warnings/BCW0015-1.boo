"""
BCW0015-1.boo(8,31): BCW0015: WARNING: Unreachable code detected.
"""
class Test:
	def WithUnreachableCode():
		WithoutUnreachableCode()
		return
		WithoutUnreachableCode()

	private def WithoutUnreachableCode():
		pass
