"""
BCW0015-3.boo(11,39): BCW0015: WARNING: Unreachable code detected.
BCW0015-3.boo(15,39): BCW0015: WARNING: Unreachable code detected.
"""
class Test:
	def WithUnreachableCode(x as int):
		if x > 0:
			for i in range(0,10):
				WithoutUnreachableCode()
				break
				WithoutUnreachableCode()
			for i in range(0,10):
				WithoutUnreachableCode()
				continue
				WithoutUnreachableCode()

	private def WithoutUnreachableCode():
		pass
