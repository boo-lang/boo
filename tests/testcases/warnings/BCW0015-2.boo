"""
BCW0015-2.boo(11,39): BCW0015: WARNING: Unreachable code detected.
BCW0015-2.boo(15,39): BCW0015: WARNING: Unreachable code detected.
"""
class Test:
	def WithUnreachableCode(x as int):
		if x > 0:
			try:
				WithoutUnreachableCode()
				raise System.Exception()
				WithoutUnreachableCode()
			except:
				WithoutUnreachableCode()
				raise System.Exception()
				WithoutUnreachableCode()

	private def WithoutUnreachableCode():
		pass
